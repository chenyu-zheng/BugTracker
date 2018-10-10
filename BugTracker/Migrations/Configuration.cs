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

            // For development only. In case permissions have been changed. 
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [ApplicationRolePermissions]");
            var dbPermissions = context.Permissions.ToList();
            context.Permissions.RemoveRange(dbPermissions);
            context.SaveChanges();
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('[Permissions]', RESEED, 0)");

            PermissionConfig.InitializePermissions(context, AppDataConfig.Permissions);
            RoleConfig.InitializeRoles(context, AppDataConfig.Roles);
            RoleConfig.InitializeRolePermissions(context, AppDataConfig.RolePermissions);

            new TicketConfig(
                context,
                AppDataConfig.TicketCategories,
                AppDataConfig.TicketPriorities,
                AppDataConfig.TicketStatus)
                .Init();

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
