using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Migrations
{
    public class PermissionConfig
    {
        public static IReadOnlyList<string> Permissions = new List<string>
        {
            "Edit User Roles",
            "View All Projects",
            "View Own Projects",
            "Create Projects",
            "Edit All Projects",
            "Edit Own Projects"
        };

        public static void InitializePermissions(ApplicationDbContext context)
        {
            foreach(string item in Permissions)
            {
                if (!context.Permissions.Any(p => p.Name == item))
                {
                    context.Permissions.Add(new Permission { Name = item });
                }
            }

            context.SaveChanges();
        }
    }
}