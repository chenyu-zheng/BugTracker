using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        [RegularExpression(@".*[a-zA-Z]+.*", ErrorMessage = "The {0} must contain at least one letter.")]
        public string Name { get; set; }
        [AllowHtml]
        [Required]
        [StringLength(10000, MinimumLength = 1, ErrorMessage = "The {0} cannot exceed {2} characters.")]
        public string Description { get; set; }
        public string Identifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public Project()
        {
            Created = DateTime.Now;
            Members = new HashSet<ApplicationUser>();
        }
    }
}