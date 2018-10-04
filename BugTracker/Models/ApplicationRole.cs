using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<Permission> Permissions { get; set; }

        public ApplicationRole() : base()
        {
            Permissions = new HashSet<Permission>();
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
            Permissions = new HashSet<Permission>();
        }
    }
}