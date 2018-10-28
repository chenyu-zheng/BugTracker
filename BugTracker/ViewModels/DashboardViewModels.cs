using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class DashboardViewModel
    {
        public int NumberOfMyProjects { get; set; }
        public int NumberOfProjectsTickets { get; set; }
        public int NumberOfAssignedTickets { get; set; }
        public int NumberOfCreatedTickets { get; set; }
        public List<TicketUpdateViewModel> TicketUpdates { get; set; }
        public List<TicketViewModel> AssignedTickets { get; set; }

        public DashboardViewModel()
        {
            TicketUpdates = new List<TicketUpdateViewModel>();
            AssignedTickets = new List<TicketViewModel>();
        }
    }

    public class TicketUpdateViewModel : TicketRevisionViewModel
    {
        public int TicketId { get; set; }
        public string TicketSubject { get; set; }
    }
}