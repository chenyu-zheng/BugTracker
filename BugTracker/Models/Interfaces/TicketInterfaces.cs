using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Interfaces
{
    public interface ITicketItem
    {
        int ProjectId { get; set; }
        string AssigneeId { get; set; }
        string AuthorId { get; set; }
    }
}