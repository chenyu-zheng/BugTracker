using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public TicketCategory()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}