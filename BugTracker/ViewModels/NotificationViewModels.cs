using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class NotificationViewModels
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool IsRead { get; set; }
    }
}