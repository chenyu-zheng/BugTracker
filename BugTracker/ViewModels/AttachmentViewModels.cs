using BugTracker.Models;
using BugTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class AttachmentViewModel : IAttachmentItem
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool CanDelete { get; set; }

        public AttachmentViewModel()
        {
            CanDelete = false;
        }
    }
}