using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class UserManageHelper
    {
        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;

        public UserManageHelper(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public Dictionary<string, bool> RoleDictionary(string userId)
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = userManager.GetRoles(userId).ToList();

            var results = new Dictionary<string, bool>();
            roles.ForEach(r => results.Add(r, userRoles.Contains(r)));

            return results;
        }
    }
}