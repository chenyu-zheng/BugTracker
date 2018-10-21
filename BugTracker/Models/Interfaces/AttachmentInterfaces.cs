using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Interfaces
{
    public interface IAttachmentItem
    {
        int TicketId { get; set; }
        string AuthorId { get; set; }
    }
}