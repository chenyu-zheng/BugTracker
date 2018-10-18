using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Ticket);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Subject");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Content,TicketId")] CreateCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == model.TicketId);
            if (ticket == null)
            {
                var error = "TicketId does not exist!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanEditTicket(userId, ticket))
            {
                var error = "Permission Denied!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var comment = new Comment
            {
                Content = model.Content,
                TicketId = model.TicketId,
                AuthorId = userId
            };
            db.Comments.Add(comment);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", comment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Subject", comment.TicketId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,Created,Updated,TicketId,AuthorId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", comment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Subject", comment.TicketId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult CommentList(int ticketId)
        {
            var ticket = db.Tickets
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }

            IMapper mapper = new Mapper(MappingConfig.Config);
            var comments = mapper.Map<ICollection<Comment>, List<CommentItemViewModel>>
                (ticket.Comments.OrderByDescending(c => c.Created).ToList());
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            comments.ForEach(c => 
            {
                c.CanEdit = uHelper.CanEditComment(userId, c.Id);
                c.CanEdit = uHelper.HasPermission(userId, "Delete Comment");
            });

            var data = new CommentViewModel
            {
                TicketId = ticketId,
                CanCreate = uHelper.CanEditTicket(userId, ticket),
                Comments = comments
            };

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
