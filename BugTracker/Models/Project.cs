using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }

        public Project()
        {
            Members = new HashSet<ApplicationUser>();
        }
    }
}