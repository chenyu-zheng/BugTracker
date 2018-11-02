using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace BugTracker.Migrations
{
    public class UserConfig
    {
        private ApplicationDbContext context;

        public UserConfig(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Init()
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));

            ApplicationUser user = null;
            string userName, displayName, password;

            // Default admin
            userName = WebConfigurationManager.AppSettings["admin-username"];
            displayName = WebConfigurationManager.AppSettings["admin-displayname"];
            password = WebConfigurationManager.AppSettings["admin-password"];
            if (!context.Users.Any(p => p.UserName == userName))
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DisplayName = displayName
                };
                userManager.Create(user, password);
            }
            else
            {
                user = context.Users.Where(p => p.UserName == userName)
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(user.Id, "Admin"))
            {
                userManager.AddToRole(user.Id, "Admin");
            }


            // Demo users:

            userName = WebConfigurationManager.AppSettings["demo-admin-username"];
            displayName = WebConfigurationManager.AppSettings["demo-admin-displayname"];
            password = WebConfigurationManager.AppSettings["demo-admin-password"];
            if (!context.Users.Any(p => p.UserName == userName))
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DisplayName = displayName
                };
                userManager.Create(user, password);
            }
            else
            {
                user = context.Users.Where(p => p.UserName == userName)
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(user.Id, "Admin"))
            {
                userManager.AddToRole(user.Id, "Admin");
            }

            userName = WebConfigurationManager.AppSettings["demo-pmanager-username"];
            displayName = WebConfigurationManager.AppSettings["demo-pmanager-displayname"];
            password = WebConfigurationManager.AppSettings["demo-pmanager-password"];
            if (!context.Users.Any(p => p.UserName == userName))
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DisplayName = displayName
                };
                userManager.Create(user, password);
            }
            else
            {
                user = context.Users.Where(p => p.UserName == userName)
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(user.Id, "Project Manager"))
            {
                userManager.AddToRole(user.Id, "Project Manager");
            }

            userName = WebConfigurationManager.AppSettings["demo-developer-username"];
            displayName = WebConfigurationManager.AppSettings["demo-developer-displayname"];
            password = WebConfigurationManager.AppSettings["demo-developer-password"];
            if (!context.Users.Any(p => p.UserName == userName))
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DisplayName = displayName
                };
                userManager.Create(user, password);
            }
            else
            {
                user = context.Users.Where(p => p.UserName == userName)
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(user.Id, "Developer"))
            {
                userManager.AddToRole(user.Id, "Developer");
            }

            userName = WebConfigurationManager.AppSettings["demo-submitter-username"];
            displayName = WebConfigurationManager.AppSettings["demo-submitter-displayname"];
            password = WebConfigurationManager.AppSettings["demo-submitter-password"];
            if (!context.Users.Any(p => p.UserName == userName))
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    DisplayName = displayName
                };
                userManager.Create(user, password);
            }
            else
            {
                user = context.Users.Where(p => p.UserName == userName)
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(user.Id, "Submitter"))
            {
                userManager.AddToRole(user.Id, "Submitter");
            }
        }
    }
}