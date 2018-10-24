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
        public List<TicketUpdateViewModel> TicketUpdates { get; set; }
        public List<TicketViewModel> NewAssignedTickets { get; set; }

        public DashboardViewModel()
        {
            TicketUpdates = new List<TicketUpdateViewModel>();
            NewAssignedTickets = new List<TicketViewModel>();
        }
    }

    public class TicketUpdateViewModel : TicketRevisionViewModel
    {
        public int TicketId { get; set; }
        public string TicketSubject { get; set; }
    }
}