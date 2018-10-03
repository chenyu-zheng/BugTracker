using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketRevision
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<TicketRevisionDetail> Details { get; set; }

        public TicketRevision()
        {
            Details = new HashSet<TicketRevisionDetail>();
        }
    }
}