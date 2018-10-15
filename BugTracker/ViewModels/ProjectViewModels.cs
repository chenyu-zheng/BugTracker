using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public bool Archived { get; set; }
        [Display(Name = "Members")]
        public int NumberOfMembers { get; set; }
        [Display(Name = "Tickets")]
        public int NumberOfTickets { get; set; }

        public ProjectViewModel()
        {
            Archived = false;
        }
    }

    public class ChangeMemberViewModel
    {
        public ProjectViewModel Project { get; set; }
        public List<UserRoleViewModel> Members { get; set; }
        public List<UserRoleViewModel> Users { get; set; }
    }
}