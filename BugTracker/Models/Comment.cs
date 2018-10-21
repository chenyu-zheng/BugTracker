using BugTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Comment : ICommentItem
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public Comment()
        {
            Created = DateTimeOffset.Now;
        }
    }
}