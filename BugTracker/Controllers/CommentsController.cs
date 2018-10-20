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
using BugTracker.ActionFilters;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Edit All Tickets, Edit Projects Tickets, Edit Assigned Tickets, Edit Created Tickets")]
        public async Task<JsonResult> Create([Bind(Include = "Content,TicketId")] CreateCommentViewModel model)
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
            var nHelper = new NotificationHelper(db);
            await nHelper.NotifyTicketCommentAsync(userId, ticket, comment);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Edit All Comments, Edit Projects Comments, Edit Created Comments")]
        public JsonResult Edit([Bind(Include = "Id,Content")] EditCommentViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var commentDb = db.Comments
                .FirstOrDefault(c => c.Id == comment.Id);
            if (comment == null)
            {
                var error = "Comment not found!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanEditComment(userId, comment.Id))
            {
                var error = "Permission Denied!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            commentDb.Content = comment.Content;
            commentDb.Updated = DateTimeOffset.Now;
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Edit All Comments, Edit Projects Comments, Edit Created Comments")]
        public JsonResult DeleteConfirmed(int id)
        {
            var comment = db.Comments
                .FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                var error = "Comment not found!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanEditComment(userId, id))
            {
                var error = "Permission Denied!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            db.Comments.Remove(comment);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [PermissionAuthorize("Edit All Tickets, Edit Projects Tickets, Edit Assigned Tickets, Edit Created Tickets")]
        public JsonResult CommentList(int ticketId)
        {
            var ticket = db.Tickets
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                return Json(new { error = "Ticket not found!" }, JsonRequestBehavior.AllowGet);
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanEditTicket(userId, ticket))
            {
                var error = "Permission Denied!";
                return Json(new { error }, JsonRequestBehavior.AllowGet);
            }
            IMapper mapper = new Mapper(MappingConfig.Config);
            var comments = mapper.Map<ICollection<Comment>, List<CommentItemViewModel>>
                (ticket.Comments.OrderByDescending(c => c.Created).ToList());
            comments.ForEach(c => c.CanEdit = uHelper.CanEditComment(userId, c.Id));
            var data = new CommentViewModel
            {
                TicketId = ticketId,
                CanCreate = true,
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
