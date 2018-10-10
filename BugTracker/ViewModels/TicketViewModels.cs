using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        [Display(Name = "Author")]
        public string AuthorName { get; set; }
        [Display(Name = "Assignee")]
        public string AssigneeName { get; set; }
        [Display(Name = "Revisions")]
        public int NumberOfRevisions { get; set; }
        [Display(Name = "Attachments")]
        public int NumberOfAttachments { get; set; }
        [Display(Name = "Comments")]
        public int NumberOfComments { get; set; }
    }

    public class TicketDetailsViewModel : TicketViewModel
    {
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public int ProjectId { get; set; }

    }
}