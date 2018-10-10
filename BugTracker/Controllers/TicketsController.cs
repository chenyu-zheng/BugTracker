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

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets
                .Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    Subject = t.Subject,
                    Created = t.Created,
                    Updated = t.Updated,
                    ProjectName = t.Project.Name,
                    Category = t.Category.Name,
                    Status = t.Status.Name,
                    Priority = t.Priority.Name,
                    AuthorName = t.Author.DisplayName,
                    AssigneeName = t.Assignee.DisplayName,
                    NumberOfRevisions = t.Revisions.Count(),
                    NumberOfComments = t.Comments.Count(),
                    NumberOfAttachments = t.Attachments.Count()
                });
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [PermissionAuthorize("Create Tickets")]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.CategoryId = new SelectList(db.TicketCategories, "Id", "Name");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.ProjectId = new SelectList(
                db.Projects.Where(p => p.Members.Any(m => m.Id == userId)),
                "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Subject,Description,ProjectId,CategoryId,PriorityId")] Ticket ticket)
        {
            var userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.TicketCategories, "Id", "Name", ticket.CategoryId);
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
                ViewBag.ProjectId = new SelectList(
                    db.Projects.Where(p => p.Members.Any(m => m.Id == userId)),
                    "Id", "Name");
                return View(ticket);
            }

            if (!db.TicketCategories.Any(p => p.Id == ticket.CategoryId) || 
                !db.TicketPriorities.Any(p => p.Id == ticket.PriorityId) ||
                !db.Projects.Any(p => p.Id == ticket.ProjectId && p.Members.Any(m => m.Id == userId)))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ticket.Status = new TicketStatus { Name = "New" };
            ticket.AuthorId = userId;
            db.Tickets.Add(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssigneeId);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticket.AuthorId);
            ViewBag.CategoryId = new SelectList(db.TicketCategories, "Id", "Name", ticket.CategoryId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject,Description,Created,Updated,StartDate,DueDate,ProjectId,CategoryId,StatusId,PriorityId,AuthorId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssigneeId);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticket.AuthorId);
            ViewBag.CategoryId = new SelectList(db.TicketCategories, "Id", "Name", ticket.CategoryId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
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
