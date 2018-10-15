using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Interfaces
{
    public interface IUserItem
    {
        string Id { get; set; }
        string UserName { get; set; }
        string DisplayName { get; set; }
    }
}