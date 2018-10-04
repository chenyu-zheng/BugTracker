using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationRole> Roles { get; set; }

        public Permission()
        {
            Roles = new HashSet<ApplicationRole>();
        }
    }
}