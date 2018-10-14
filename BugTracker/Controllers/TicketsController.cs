using System;
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

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var model = db.Tickets
                .ProjectTo<TicketViewModel>(MappingConfig.Config)
                .ToList();
            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            foreach (var item in model)
            {
                if (User.IsInRole("Admin") ||
                    User.IsInRole("Project Manager") && helper.IsProjectMember(userId, item.ProjectId) ||
                    User.IsInRole("Developer") && item.AssigneeId == userId ||
                    User.IsInRole("Submitter") && item.AuthorId == userId)
                {
                    item.CanViewDetails = true;
                }
            }
            return View(model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IQueryable<Ticket> query;
            if (User.IsInRole("Admin"))
            {
                query = db.Tickets.AsQueryable();
            }
            else
            {
                query = Enumerable.Empty<Ticket>().AsQueryable();
                var user = new UserManageHelper().FindUserById(User.Identity.GetUserId());
                if (User.IsInRole("Project Manager"))
                {
                    query = query.Union(user.Projects.SelectMany(p => p.Tickets)).AsQueryable();
                }
                if (User.IsInRole("Developer"))
                {
                    query = query.Union(user.AssignedTickets).AsQueryable();
                }
                if (User.IsInRole("Submitter"))
                {
                    query = query.Union(user.Tickets).AsQueryable();
                }
            }

            var ticket = query
                //.ProjectTo<TicketDetailsViewModel>(MappingConfig.Config)
                .FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            IMapper mapper = new Mapper(MappingConfig.Config);
            var model = mapper.Map<Ticket, TicketDetailsViewModel>(ticket);
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
                model.ProjectList = new SelectList(db.Projects
                    .Where(p => p.Members.Any(m => m.Id == userId))
                    .Select(p => new { p.Name, p.Id }),
                    "Id", "Name");
                model.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
                model.CategoryList = new SelectList(db.TicketCategories, "Id", "Name");
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
        [PermissionAuthorize("Edit Tickets")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            Ticket ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null ||
                !(User.IsInRole("Admin") ||
                    User.IsInRole("Submitter") && ticket.AuthorId == userId ||
                    User.IsInRole("Developer") && ticket.AssigneeId == userId ||
                    User.IsInRole("Project Manager") && helper.IsProjectMember(userId, ticket.ProjectId))
                )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IMapper mapper = new Mapper(MappingConfig.Config);
            var model = mapper.Map<Ticket, EditTicketViewModel>(ticket);
            var statusQuery = db.TicketStatus.AsQueryable();
            if (ticket.AssigneeId == null)
            {
                statusQuery = statusQuery.Where(s => s.Name != "Assigned");
            }
            else
            {
                statusQuery = statusQuery.Where(s => s.Name != "New");
            }
            model.ProjectList = new SelectList(db.Projects.Where(p => p.Id == model.ProjectId), "Id", "Name");
            model.CategoryList = new SelectList(db.TicketCategories, "Id", "Name");
            model.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
            model.StatusList = new SelectList(statusQuery, "Id", "Name");

            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Edit Tickets")]
        public ActionResult Edit([Bind(Include = "Id,Subject,Description,ProjectId,CategoryId,StatusId,PriorityId,AssigneeId")] EditTicketViewModel model)
        {
            ActionResult modelInvalid()
            {
                model.ProjectList = new SelectList(db.Projects.Where(p => p.Id == model.ProjectId), "Id", "Name");
                model.CategoryList = new SelectList(db.TicketCategories, "Id", "Name");
                model.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
                if (db.Tickets
                .Where(t => t.Id == model.Id)
                .Select(t => t.AssigneeId)
                .FirstOrDefault() == null)
                {
                    model.StatusList = new SelectList(db.TicketStatus.Where(s => s.Name != "Assigned"), "Id", "Name");
                }
                else
                {
                    model.StatusList = new SelectList(db.TicketStatus.Where(s => s.Name != "New"), "Id", "Name");
                }
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return modelInvalid();
            }
            var userId = User.Identity.GetUserId();
            var helper = new UserManageHelper();
            var ticketDb = db.Tickets.FirstOrDefault(t => t.Id == model.Id);
            if (ticketDb == null ||
                !(User.IsInRole("Admin") ||
                    User.IsInRole("Submitter") && ticketDb.AuthorId == userId ||
                    User.IsInRole("Developer") && ticketDb.AssigneeId == userId ||
                    User.IsInRole("Project Manager") && helper.IsProjectMember(userId, ticketDb.ProjectId))
                )
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
            if (User.IsInRole("Admin") ||
                User.IsInRole("Project Manager") && helper.IsProjectMember(userId, ticketDb.ProjectId))
            {
                ticketDb.StatusId = model.StatusId;
            }
            ticketDb.Updated = DateTime.Now;
            db.Entry(ticketDb).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = ticketDb.Id });
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                if (!(string.IsNullOrWhiteSpace(assigneeId) ||           // Assignee doesn't exist or isn't a developer or not belong to the project
                        helper.HasRole(assigneeId, "Developer") &&
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
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id });
        }

        [PermissionAuthorize("Assign Tickets")]
        public JsonResult GetAssigneeList(int projectId)
        {
            var roleId = db.Roles
                .Where(r => r.Name == "Developer")
                .Select(r => r.Id)
                .FirstOrDefault();
            var data = db.Users
                .Where(u =>
                    u.Projects.Any(p => p.Id == projectId) &&
                    u.Roles.Any(ur => ur.RoleId == roleId))
                .Select(u => new { u.Id, u.DisplayName })
                .ToList();
            if (data.Any())
            {
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
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
