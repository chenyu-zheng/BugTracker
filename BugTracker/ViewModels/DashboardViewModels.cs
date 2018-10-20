using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class DashboardViewModel
    {
        public int MyProjects { get; set; }
        public int ProjectsTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int CreatedTickets { get; set; }
    }
}