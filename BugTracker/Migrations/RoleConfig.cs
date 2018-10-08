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
        

        public static void InitializeRoles(ApplicationDbContext context, IEnumerable<string> roles)
        {
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            foreach (string item in roles)
            {
                if (!context.Roles.Any(r => r.Name == item))
                {
                    roleManager.Create(new ApplicationRole { Name = item });
                }
            }
        }

        public static void InitializeRolePermissions(ApplicationDbContext context, IReadOnlyDictionary<string, IReadOnlyList<string>> rolePermissions)
        {
            foreach(var item in rolePermissions)
            {
                var role = context.Roles
                    .FirstOrDefault(r => r.Name == item.Key);

                var permissions = context.Permissions
                    .Where(p => item.Value.Contains(p.Name))
                    .ToList();

                foreach (var permission in permissions)
                {
                    if (!role.Permissions.Any(p => p.Id == permission.Id))
                    {
                        role.Permissions.Add(permission);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}