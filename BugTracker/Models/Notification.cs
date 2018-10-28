using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string ItemType { get; set; }
        public string ItemId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsRead { get; set; }

        public Notification()
        {
            IsRead = false;
        }
    }
}