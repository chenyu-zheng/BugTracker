using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Migrations
{
    public class PermissionConfig
    {
        public static void InitializePermissions(ApplicationDbContext context, IEnumerable<string> permissions)
        {
            foreach (string item in permissions)
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