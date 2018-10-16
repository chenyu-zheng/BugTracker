﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.ActionFilters;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using BugTracker.Helpers;
using BugTracker.Models.Interfaces;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [PermissionAuthorize("List All Tickets")]
        public ActionResult Index()
        {
            var model = db.Tickets
                .ProjectTo<TicketViewModel>(MappingConfig.Config)
                .ToList();

            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            model.ForEach(m => m.CanEdit = helper.CanEditTicket(userId, m));

            ViewBag.Type = "All";
            return View(model);
        }

        [PermissionAuthorize("List Projects Tickets")]
        public ActionResult FromMyprojects()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Tickets
                .Where(t => t.Project.Members.Any(m => m.Id == userId))
                .ProjectTo<TicketViewModel>(MappingConfig.Config)
                .ToList();

            var helper = new UserManageHelper();
            model.ForEach(m => m.CanEdit = helper.CanEditTicket(userId, m));

            ViewBag.Type = "From My Projects";
            return View("Index", model);
        }

        [PermissionAuthorize("List Assigned Tickets")]
        public ActionResult AssignedToMe()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Tickets
                .Where(t => t.AssigneeId == userId)
                .ProjectTo<TicketViewModel>(MappingConfig.Config)
                .ToList();

            var helper = new UserManageHelper();
            model.ForEach(m => m.CanEdit = helper.CanEditTicket(userId, m));

            ViewBag.Type = "Assigned to Me";
            return View("Index", model);
        }

        [PermissionAuthorize("List Created Tickets")]
        public ActionResult CreatedByMe()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Tickets
                .Where(t => t.AuthorId == userId)
                .ProjectTo<TicketViewModel>(MappingConfig.Config)
                .ToList();

            var helper = new UserManageHelper();
            model.ForEach(m => m.CanEdit = helper.CanEditTicket(userId, m));

            ViewBag.Type = "Posted by Me";
            return View("Index", model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = db.Tickets
                .Where(t => t.Id == id)
                .ProjectTo<TicketDetailsViewModel>(MappingConfig.Config)
                .FirstOrDefault();

            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            if (model == null || !helper.CanEditTicket(userId, model))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.CanEdit = true;
            model.CanAssign = helper.HasPermission(userId, "Assign Tickets");
            model.CanDelete = helper.HasPermission(userId, "Delete Tickets");
            return View(model);
        }

        // GET: Tickets/Create
        [PermissionAuthorize("Create Tickets")]
        public ActionResult Create(int? projectId)
        {
            var userId = User.Identity.GetUserId();

            var projects = db.Projects
                .Where(p => p.Members.Any(m => m.Id == userId))
                .Select(p => new { p.Name, p.Id });

            if (!projects.Any())
            {
                ViewBag.ErrorMessage = "You are not a member of any project.";
                return View("Error");
            }
            if (projectId != null && !projects.Any(p => p.Id == projectId))
            {
                return HttpNotFound();
            }

            var priorities = db.TicketPriorities.ToList();
            var categories = db.TicketCategories.ToList();

            var model = new CreateTicketViewModel()
            {
                ProjectId = projectId,
                ProjectList = new SelectList(projects, "Id", "Name"),

                PriorityId = priorities.FirstOrDefault().Id,
                PriorityList = new SelectList(priorities, "Id", "Name"),

                CategoryId = categories.FirstOrDefault().Id,
                CategoryList = new SelectList(categories, "Id", "Name")
            };
            model.CanAssign = new UserManageHelper().HasPermission(userId, "Assign Tickets");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Create Tickets")]
        public ActionResult Create([Bind(Include = "Subject,Description,ProjectId,CategoryId,PriorityId,AssigneeId")] CreateTicketViewModel model)
        {
            var userId = User.Identity.GetUserId();

            ActionResult modelInvalid()
            {
                var vMHelper = new ViewModelHelper();
                model = vMHelper.AddSelectLists(model, userId);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return modelInvalid();
            }
            if (db.Tickets.Any(p => p.Subject == model.Subject))
            {
                ModelState.AddModelError(nameof(model.Subject), $"The {nameof(model.Subject)} must be unique");
                return modelInvalid();
            }

            IMapper mapper = new Mapper(MappingConfig.Config);
            var ticket = mapper.Map<CreateTicketViewModel, Ticket>(model);

            var helper = new UserManageHelper();
            if (!db.TicketCategories.Any(p => p.Id == ticket.CategoryId) || // CategoryId doesn't exist
                !db.TicketPriorities.Any(p => p.Id == ticket.PriorityId) || // PriorityId doesn't exist
                !db.Projects.Any(p => p.Id == ticket.ProjectId &&           // ProjectId doesn't exist or the author is not a member
                    p.Members.Any(m => m.Id == userId)) ||
                !(string.IsNullOrWhiteSpace(ticket.AssigneeId) ||           // Assignee doesn't exist or isn't a developer or not belong to the project
                    helper.HasRole(ticket.AssigneeId, "Developer") &&
                    helper.IsProjectMember(ticket.AssigneeId, ticket.ProjectId))
                )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var statusName = string.IsNullOrWhiteSpace(ticket.AssigneeId) ? "New" : "Assigned";
            ticket.StatusId = db.TicketStatus
                .Where(s => s.Name == statusName)
                .Select(s => s.Id)
                .FirstOrDefault();

            ticket.AuthorId = userId;
            db.Tickets.Add(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Tickets/Edit/5
        [PermissionAuthorize("Edit All Tickets, Edit Projects Tickets, Edit Assigned Tickets, Edit Created Tickets")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            Ticket ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null || !helper.CanEditTicket(userId, ticket))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IMapper mapper = new Mapper(MappingConfig.Config);
            var model = mapper.Map<Ticket, EditTicketViewModel>(ticket);
            var vMHelper = new ViewModelHelper();
            model = vMHelper.AddSelectLists(model);
            model.CanEditStatus = helper.HasPermission(userId, "Edit Ticket Status");
            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Edit All Tickets, Edit Projects Tickets, Edit Assigned Tickets, Edit Created Tickets")]
        public ActionResult Edit([Bind(Include = "Id,Subject,Description,ProjectId,CategoryId,StatusId,PriorityId,AssigneeId")] EditTicketViewModel model)
        {
            ActionResult modelInvalid()
            {
                var vMHelper = new ViewModelHelper();
                model = vMHelper.AddSelectLists(model);
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return modelInvalid();
            }
            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            var ticketDb = db.Tickets.FirstOrDefault(t => t.Id == model.Id);
            if (ticketDb == null || !helper.CanEditTicket(userId, ticketDb))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (model.Subject != ticketDb.Subject && db.Tickets.Any(t => t.Subject == model.Subject))
            {
                ModelState.AddModelError(nameof(model.Subject), $"The {nameof(model.Subject)} must be unique");
                return modelInvalid();
            }
            if (!db.TicketCategories.Any(p => p.Id == model.CategoryId) || // CategoryId doesn't exist
                !db.TicketPriorities.Any(p => p.Id == model.PriorityId) || // PriorityId doesn't exist
                !db.TicketStatus.Any(p => p.Id == model.StatusId)          // StatusId doesn't exist
                )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ticketDb.Subject = model.Subject;
            ticketDb.Description = model.Description;
            ticketDb.CategoryId = model.CategoryId;
            ticketDb.PriorityId = model.PriorityId;
            if (helper.HasPermission(userId, "Edit Ticket Status"))
            {
                ticketDb.StatusId = model.StatusId;
            }
            ticketDb.Updated = DateTimeOffset.Now;
            var revHelper = new TicketRevisionHelper(db);
            var revision = revHelper.CreateRevision(ticketDb, userId);
            if (revision != null)
            {
                db.TicketRevisions.Add(revision);
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { id = ticketDb.Id });
        }

        // GET: Tickets/Delete/5
        [PermissionAuthorize("Delete Tickets")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.Tickets
                .Where(t => t.Id == id)
                .ProjectTo<TicketDetailsViewModel>(MappingConfig.Config)
                .FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Delete Tickets")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Assign Tickets")]
        public ActionResult Assign(int? id, string assigneeId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticket = db.Tickets
                .Where(t => t.Id == id)
                .Include(t => t.Status)
                .FirstOrDefault();
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(assigneeId))
            {
                assigneeId = null;
            }
            if (ticket.AssigneeId != assigneeId)
            {
                var helper = new UserManageHelper();
                if (!(string.IsNullOrWhiteSpace(assigneeId) ||
                        helper.HasPermission(assigneeId, "Receive Tickets") &&
                        helper.IsProjectMember(assigneeId, ticket.ProjectId)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ticket.AssigneeId = assigneeId;
                if (ticket.Status.Name == "Assigned" && assigneeId == null)
                {
                    ticket.Status = db.TicketStatus.FirstOrDefault(s => s.Name == "New");
                }
                if (ticket.Status.Name == "New" && assigneeId != null)
                {
                    ticket.Status = db.TicketStatus.FirstOrDefault(s => s.Name == "Assigned");
                }
                ticket.Updated = DateTime.Now;
                var revHelper = new TicketRevisionHelper(db);
                var revision = revHelper.CreateRevision(ticket, User.Identity.GetUserId());
                if (revision != null)
                {
                    db.TicketRevisions.Add(revision);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id });
        }

        [PermissionAuthorize("Assign Tickets")]
        public JsonResult GetAssigneeList(int projectId)
        {
            var roleIds = db.Roles
                .Where(r => r.Permissions.Any(p => p.Name == "Receive Tickets"))
                .Select(r => r.Id);
            if (!roleIds.Any())
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
            var users = db.Users
                .Where(u =>
                    u.Projects.Any(p => p.Id == projectId) &&
                    u.Roles.Any(ur => roleIds.Contains(ur.RoleId)))
                .Select(u => new { u.Id, u.DisplayName })
                .ToList();
            if (!users.Any())
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
            var helper = new UserManageHelper();
            var data = users.Select(u => new { u.Id, u.DisplayName, Roles = helper.RoleList(u.Id) });
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
