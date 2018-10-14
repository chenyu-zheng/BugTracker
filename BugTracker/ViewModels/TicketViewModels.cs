using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public string AssigneeId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    public class AttachmentViewModel
    {

    }

    public class CommentViewModel
    {

    }

    public class CreateTicketViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Subject { get; set; }
        [AllowHtml]
        [Required]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "The {0} cannot exceed {2} characters.")]
        public string Description { get; set; }
        [Required]
        public int? ProjectId { get; set; }
        public IEnumerable<SelectListItem> ProjectList { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public int PriorityId { get; set; }
        public IEnumerable<SelectListItem> PriorityList { get; set; }
        [Display(Name = "Assign To")]
        public string AssigneeId { get; set; }
        public IEnumerable<SelectListItem> AssigneeList { get; set; }

        public CreateTicketViewModel()
        {
            AssigneeList = new HashSet<SelectListItem>();
        }
    }

    public class EditTicketViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Subject { get; set; }
        [AllowHtml]
        [Required]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "The {0} cannot exceed {2} characters.")]
        public string Description { get; set; }
        [Required]
        public int? ProjectId { get; set; }
        public IEnumerable<SelectListItem> ProjectList { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public int PriorityId { get; set; }
        public IEnumerable<SelectListItem> PriorityList { get; set; }
        public int StatusId { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }
}