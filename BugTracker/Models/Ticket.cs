using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int CategoryId { get; set; }
        public virtual TicketCategory Category { get; set; }
        public int StatusId { get; set; }
        public virtual TicketStatus Status { get; set; }
        public int PriorityId { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public string AssigneeId { get; set; }
        public virtual ApplicationUser Assignee { get; set; }
        public virtual ICollection<TicketRevision> Revisions { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public Ticket()
        {
            Created = DateTimeOffset.Now;
            Revisions = new HashSet<TicketRevision>();
            Attachments = new HashSet<Attachment>();
            Comments = new HashSet<Comment>();
        }
    }
}