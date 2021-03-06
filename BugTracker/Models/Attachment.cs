﻿using BugTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Attachment : IAttachmentItem
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public Attachment()
        {
            Created = DateTimeOffset.Now;
        }
    }
}