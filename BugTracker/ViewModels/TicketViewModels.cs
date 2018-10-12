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
        [Display(Name = "Last Updated")]
        public DateTimeOffset LastUpdated { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
        [Display(Name = "Status")]
        public string StatusName { get; set; }
        [Display(Name = "Priority")]
        public string PriorityName { get; set; }
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
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public int ProjectId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    public class AttachmentViewModel
    {

    }

    public class CommentViewModel
    {

    }
}