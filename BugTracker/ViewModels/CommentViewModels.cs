using BugTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class CommentViewModel
    {
        public int TicketId { get; set; }
        public List<CommentItemViewModel> Comments { get; set; }
        public bool CanCreate { get; set; }

        public CommentViewModel()
        {
            Comments = new List<CommentItemViewModel>();
            CanCreate = false;
        }
    }

    public class CreateCommentViewModel
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "The {0} cannot exceed {2} characters.")]
        public string Content { get; set; }
        [Required]
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
    }

    public class EditCommentViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "The {0} cannot exceed {2} characters.")]
        public string Content { get; set; }
    }

    public class CommentItemViewModel : ICommentItem
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorDisplayName { get; set; }
        public bool CanEdit { get; set; }

        public CommentItemViewModel()
        {
            CanEdit = false;
        }
    }
}