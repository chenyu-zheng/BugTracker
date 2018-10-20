using BugTracker.Models;
using BugTracker.Models.Interfaces;
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

        public ApplicationUser FindUserById(string userId)
        {
            return userManager.FindById(userId);
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

        public void ResetRole(string userId, string roleName)
        {
            var roles = userManager.GetRoles(userId).ToArray();
            userManager.RemoveFromRoles(userId, roles);
            userManager.AddToRole(userId, roleName);
        }

        public bool HasRole(string userId, string roleName)
        {
            try
            {
                return userManager.GetRoles(userId).Contains(roleName);
            }
            catch
            {
                return false;
            }
        }

        public bool HasPermission(string userId, string permission)
        {
            try
            {
                var userRoles = RoleList(userId);
                var userPermissions = db.Roles
                    .Where(r => userRoles.Contains(r.Name))
                    .SelectMany(r => r.Permissions, (r, p) => p.Name)
                    .Distinct()
                    .ToList();

                return userPermissions.Contains(permission);
            }
            catch
            {
                return false;
            }
        }

        public bool IsProjectMember(string userId, int projectId)
        {
            try
            {
                return db.Users
                    .Any(u => u.Id == userId &&
                        u.Projects.Any(p => p.Id == projectId));
            }
            catch
            {
                return false;
            }
        }

        public bool CanEditTicket(string userId, ITicketItem ticket)
        {
            if (HasPermission(userId, "Edit All Tickets") ||
                    HasPermission(userId, "Edit Projects Tickets") && IsProjectMember(userId, ticket.ProjectId) ||
                    HasPermission(userId, "Edit Assigned Tickets") && ticket.AssigneeId == userId ||
                    HasPermission(userId, "Edit Created Tickets") && ticket.AuthorId == userId)
            {
                return true;
            }
            return false;
        }

        public bool CanEditComment(string userId, int commentId)
        {
            return HasPermission(userId, "Edit All Comments") ||
                   HasPermission(userId, "Edit Created Comments") && 
                        db.Comments.Any(c => c.Id == commentId && c.AuthorId == userId) ||
                   HasPermission(userId, "Edit Projects Comments") &&
                        db.Comments.Any(c => c.Id == commentId && c.Ticket.Project.Members.Any(m => m.Id == userId));
                
        }
    }
}