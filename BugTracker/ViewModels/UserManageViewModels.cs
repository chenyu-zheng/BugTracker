using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Name")]
        public string DisplayName { get; set; }
    }

    public class UserRoleViewModel : UserViewModel
    {
        public Dictionary<string, bool> Roles { get; set; }
    }

    public class UserProfileViewModel : UserViewModel
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        [Display(Name = "Assigned Projects")]
        public int AssignedProjects { get; set; }
        [Display(Name = "Created Tickets")]
        public int CreatedTickets { get; set; }
        [Display(Name = "Assigned Tickets")]
        public int AssignedTickets { get; set; }
    }
}