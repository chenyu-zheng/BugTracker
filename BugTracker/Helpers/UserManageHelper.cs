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

        public bool CanAssignTicket(string userId, int projectId)
        {
            return HasPermission(userId, "Assign All Tickets") ||
                   HasPermission(userId, "Assign Projects Tickets") && IsProjectMember(userId, projectId);
        }

        public bool CanBeAssignedTicket(string assigneeId, int projectId)
        {
            return HasPermission(assigneeId, "Receive Tickets") && IsProjectMember(assigneeId, projectId);
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

        public bool CanEditTicket(string userId, int ticketId)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == ticketId);
            return CanEditTicket(userId, ticket);
        }

        public bool CanEditComment(string  userId, ICommentItem comment)
        {
            return HasPermission(userId, "Edit All Comments") ||
                   HasPermission(userId, "Edit Created Comments") &&
                        comment.AuthorId == userId ||
                   HasPermission(userId, "Edit Projects Comments") &&
                        db.Tickets.Any(t => t.Id == comment.TicketId && t.Project.Members.Any(m => m.Id == userId));
        }

        public bool CanEditComment(string userId, int commentId)
        {
            var comment = db.Comments.FirstOrDefault(c => c.Id == commentId);
            return CanEditComment(userId, comment);
        }

        public bool CanDeleteAttachments(string userId, IAttachmentItem attachment)
        {
            return HasPermission(userId, "Delete All Attachments") ||
                   HasPermission(userId, "Delete Created Attachments") &&
                        attachment.AuthorId == userId ||
                   HasPermission(userId, "Delete Projects Attachments") &&
                        db.Tickets.Any(t => t.Id == attachment.TicketId && t.Project.Members.Any(m => m.Id == userId));
        }
    }
}