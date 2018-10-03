using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketRevisionDetail
    {
        public int Id { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}