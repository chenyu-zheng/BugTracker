using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

            ApplicationUser demoAdmin = null;
            if (!context.Users.Any(p => p.UserName == "demo.admin@bugtracker.com"))
            {
                demoAdmin = new ApplicationUser
                {
                    UserName = "demo.admin@bugtracker.com",
                    Email = "demo.admin@bugtracker.com",
                    DisplayName = "Demo-Admin"
                };

                userManager.Create(demoAdmin, "BugTracker-DemoAdmin");
            }
            else
            {
                demoAdmin = context.Users.Where(p => p.UserName == "demo.admin@bugtracker.com")
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(demoAdmin.Id, "Admin"))
            {
                userManager.AddToRole(demoAdmin.Id, "Admin");
            }

            ApplicationUser demoManager = null;
            if (!context.Users.Any(p => p.UserName == "demo.pmanager@bugtracker.com"))
            {
                demoManager = new ApplicationUser
                {
                    UserName = "demo.pmanager@bugtracker.com",
                    Email = "demo.pmanager@bugtracker.com",
                    DisplayName = "Demo-PManager"
                };

                userManager.Create(demoManager, "BugTracker-DemoPManager");
            }
            else
            {
                demoManager = context.Users.Where(p => p.UserName == "demo.pmanager@bugtracker.com")
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(demoManager.Id, "Project Manager"))
            {
                userManager.AddToRole(demoManager.Id, "Project Manager");
            }

            ApplicationUser demoDeveloper = null;
            if (!context.Users.Any(p => p.UserName == "demo.developer@bugtracker.com"))
            {
                demoDeveloper = new ApplicationUser
                {
                    UserName = "demo.developer@bugtracker.com",
                    Email = "demo.developer@bugtracker.com",
                    DisplayName = "Demo-Developer"
                };

                userManager.Create(demoDeveloper, "BugTracker-DemoDeveloper");
            }
            else
            {
                demoDeveloper = context.Users.Where(p => p.UserName == "demo.developer@bugtracker.com")
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(demoDeveloper.Id, "Developer"))
            {
                userManager.AddToRole(demoDeveloper.Id, "Developer");
            }

            ApplicationUser demoSubmitter = null;
            if (!context.Users.Any(p => p.UserName == "demo.submitter@bugtracker.com"))
            {
                demoSubmitter = new ApplicationUser
                {
                    UserName = "demo.submitter@bugtracker.com",
                    Email = "demo.submitter@bugtracker.com",
                    DisplayName = "Demo-Submitter"
                };

                userManager.Create(demoSubmitter, "BugTracker-DemoSubmitter");
            }
            else
            {
                demoSubmitter = context.Users.Where(p => p.UserName == "demo.submitter@bugtracker.com")
                    .FirstOrDefault();
            }
            if (!userManager.IsInRole(demoSubmitter.Id, "Submitter"))
            {
                userManager.AddToRole(demoSubmitter.Id, "Submitter");
            }
        }
    }
}