using BugTracker.ActionFilters;
using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class AttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [PermissionAuthorize("Edit All Tickets, Edit Projects Tickets, Edit Assigned Tickets, Edit Created Tickets")]
        public async Task<ActionResult> Create(int ticketId, HttpPostedFileBase file)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null || file == null || file.ContentLength == 0 || file.ContentLength > 2097152)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanEditTicket(userId, ticket))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var attachment = new Attachment
            {
                TicketId = ticketId,
                FileName = Path.GetFileName(file.FileName),
                AuthorId = userId
            };
            if (FileUploadHelper.IsWebFriendlyImage(file))
            {
                attachment.ContentType = "WebImage";
            }
            string directory = "/Uploads/";
            string saveName = FileUploadHelper.MD5String(file) + Path.GetExtension(file.FileName);
            attachment.FileUrl = directory + saveName;
            file.SaveAs(Path.Combine(Server.MapPath("~" + directory), saveName));
            db.Attachments.Add(attachment);
            var revHelper = new TicketRevisionHelper(db);
            var revision = revHelper.CreateAttachmentRevision(attachment, userId);
            if (revision != null)
            {
                db.TicketRevisions.Add(revision);
            }
            db.SaveChanges();
            if (!string.IsNullOrWhiteSpace(ticket.AssigneeId) && ticket.AssigneeId != userId)
            {
                var nHelper = new NotificationHelper(db);
                await nHelper.NotifyTicketAttachmentAsync(userId, ticket, attachment);
            }
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }

        [PermissionAuthorize("Delete All Attachments, Delete Projects Attachments, Delete Created Attachments")]
        public ActionResult Delete(int id)
        {
            var attachment = db.Attachments.FirstOrDefault(a => a.Id == id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            var uHelper = new UserManageHelper(db);
            if (!uHelper.CanDeleteAttachments(userId, attachment))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticketId = attachment.TicketId;
            var fileUrl = attachment.FileUrl;
            db.Attachments.Remove(attachment);
            var revHelper = new TicketRevisionHelper(db);
            var revision = revHelper.CreateAttachmentRevision(attachment, userId);
            if (revision != null)
            {
                db.TicketRevisions.Add(revision);
            }
            db.SaveChanges();
            if (!db.Attachments.Any(a => a.FileUrl == fileUrl))
            {
                FileUploadHelper.DeleteFile(Server.MapPath("~" + fileUrl));
            }
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
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