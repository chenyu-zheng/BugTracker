using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class UserManageHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;

        public UserManageHelper()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new ApplicationUserStore(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
        }

        public UserManageHelper(ApplicationDbContext db)
        {
            this.db = db;
            userManager = new ApplicationUserManager(new ApplicationUserStore(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
        }

        public UserManageHelper(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public List<string> RoleList(string userId)
        {
            return userManager.GetRoles(userId).ToList();
        }

        public Dictionary<string, bool> RoleDictionary(string userId)
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = userManager.GetRoles(userId).ToList();

            var results = new Dictionary<string, bool>();
            roles.ForEach(r => results.Add(r, userRoles.Contains(r)));

            return results;
        }

        public bool HasPermission(string userId, string permission)
        {
            var userRoles = RoleList(userId);
            var userPermissions = db.Roles
                .Where(r => userRoles.Contains(r.Name))
                .SelectMany(r => r.Permissions, (r, p) => p.Name)
                .Distinct()
                .ToList();

            return userPermissions.Contains(permission);
        }
    }
}