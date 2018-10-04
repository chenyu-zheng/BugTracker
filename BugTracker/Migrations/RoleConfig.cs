using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Migrations
{
    public class RoleConfig
    {
        public static IReadOnlyList<string> Roles = new List<string>
        {
            "Admin",
            "Project Manager",
            "Developer",
            "Submitter"
        };

        public static void InitializeRoles(ApplicationDbContext context)
        {
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            foreach (string item in Roles)
            {
                if (!context.Roles.Any(r => r.Name == item))
                {
                    roleManager.Create(new ApplicationRole { Name = item });
                }
            }
        }

        public static void InitializeRolePermissions(ApplicationDbContext context)
        {
            foreach (string item in PermissionConfig.Permissions)
            {
                var adminRole = context.Roles.Where(r => r.Name == "Admin").FirstOrDefault();

                var permission = context.Permissions.Where(p => p.Name == item).FirstOrDefault();

                if (!adminRole.Permissions.Any(p => p.Name == permission.Name))
                {
                    adminRole.Permissions.Add(permission);
                }
            }

            context.SaveChanges();
        }
    }
}