using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{ 
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        [InverseProperty("Author")]
        public virtual ICollection<Ticket> Tickets { get; set; }
        [InverseProperty("AssignedToUser")]
        public virtual ICollection<Ticket> AssignedTickets { get; set; }

        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            Tickets = new HashSet<Ticket>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("DisplayName", DisplayName));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public new DbSet<ApplicationRole> Roles { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}