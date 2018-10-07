using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int NumberOfMembers { get; set; }
        public int NumberOfTickets { get; set; }
    }

    public class ChangeMemberViewModel
    {
        public ProjectViewModel Project { get; set; }
        public List<UserViewModel> Members { get; set; }
        public List<UserViewModel> Users { get; set; }
    }
}