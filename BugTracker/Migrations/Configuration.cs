namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            // Initialize Permissions
            PermissionConfig.InitializePermissions(context);

            RoleConfig.InitializeRoles(context);

            RoleConfig.InitializeRolePermissions(context);


            ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));

            ApplicationUser adminUser = null;
            if (!context.Users.Any(p => p.UserName == "admin@bugtracker.com"))
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@bugtracker.com",
                    Email = "admin@bugtracker.com",
                    DisplayName = "Admin"
                };

                userManager.Create(adminUser, "bugadmin");
            }
            else
            {
                adminUser = context.Users.Where(p => p.UserName == "admin@bugtracker.com")
                    .FirstOrDefault();
            }

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }
        }
    }
}
