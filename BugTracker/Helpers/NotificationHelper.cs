using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Attachment = BugTracker.Models.Attachment;

namespace BugTracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public NotificationHelper(ApplicationDbContext db)
        {
            this.db = db;
            userManager = new ApplicationUserManager(new ApplicationUserStore(db));
        }

        public async Task SendNotification(string receiverId, string subject, string body)
        {
            try
            {
                var from = $"Notification<{WebConfigurationManager.AppSettings["emailfrom"]}>";
                var to = userManager.FindById(receiverId).Email;

                var email = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var svc = new PersonalEmailService();
                await svc.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }

        public async Task NotifyTicketAssignmentAsync(string userId, int ticketId)
        {
            var ticket = db.Tickets
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefault(t => t.Id == ticketId);
            var name = userManager.FindById(userId).DisplayName;
            var subject = "You Have Been Assigned a Ticket";
            var body = $"<h3>{name} assigned a ticket to you.</h3>" +
                       $"<p>Ticket Details:</p>" +
                       $"<h4>{ticket.Subject}</h4>" +
                       $"<p>{ticket.Description}</p>" +
                       $"<strong>Attachments</strong> {ticket.Attachments.Count()}<br />" +
                       $"<strong>Project</strong> {ticket.Project.Name}<br />" +
                       $"<strong>Author</strong> {ticket.Author.DisplayName}<br />" +
                       $"<strong>Catagory</strong> {ticket.Category.Name}<br />" +
                       $"<strong>Priority</strong> {ticket.Priority.Name}<br />" +
                       $"<strong>Status</strong> {ticket.Status.Name}";

            await SendNotification(ticket.AssigneeId, subject, body);
        }

        public async Task NotifyTicketChangeAsync(string userId, Ticket ticket)
        {
            var name = userManager.FindById(userId).DisplayName;
            var subject = "Your Ticket Has Been Modified";
            var body = $"<h3>{name} modified a ticket assigned to you.</h3>" +
                       $"<p>Ticket Details:</p>" +
                       $"<h4>{ticket.Subject}</h4>" +
                       $"<p>{ticket.Description}</p>" +
                       $"<strong>Attachments</strong> {ticket.Attachments.Count()}<br />" +
                       $"<strong>Project</strong> {ticket.Project.Name}<br />" +
                       $"<strong>Author</strong> {ticket.Author.DisplayName}<br />" +
                       $"<strong>Catagory</strong> {ticket.Category.Name}<br />" +
                       $"<strong>Priority</strong> {ticket.Priority.Name}<br />" +
                       $"<strong>Status</strong> {ticket.Status.Name}";

            await SendNotification(ticket.AssigneeId, subject, body);
        }

        public async Task NotifyTicketCommentAsync(string userId, Ticket ticket, Comment comment)
        {
            var name = userManager.FindById(userId).DisplayName;
            var subject = "Your Ticket Has a New Comment";
            var body = $"<h3>{name} posted a comment to your ticket.</h3>" +
                       $"<strong>Comment:</strong>" +
                       $"<p>{comment.Content}</p>" +
                       $"<strong>Ticket:</strong>" +
                       $"<h4>{ticket.Subject}</h4>" +
                       $"<p>{ticket.Description}</p>" +
                       $"<strong>Project:</strong>" +
                       $"<p>{ticket.Project.Name}</p>";

            await SendNotification(ticket.AssigneeId, subject, body);
        }

        public async Task NotifyTicketAttachmentAsync(string userId, Ticket ticket, Attachment attachment)
        {
            var name = userManager.FindById(userId).DisplayName;
            var subject = "Your Ticket Has a New Attachment";
            var body = $"<h3>{name} added an attachment to your ticket.</h3>" +
                       $"<strong>New Attachment:</strong>" +
                       $"<p>{attachment.FileName}</p>" +
                       $"<strong>Ticket:</strong>" +
                       $"<h4>{ticket.Subject}</h4>" +
                       $"<p>{ticket.Description}</p>" +
                       $"<strong>Project:</strong>" +
                       $"<p>{ticket.Project.Name}</p>";

            await SendNotification(ticket.AssigneeId, subject, body);
        }
    }
}