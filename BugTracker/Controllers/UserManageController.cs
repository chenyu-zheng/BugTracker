using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly ApplicationUserManager userManager;
        private readonly ApplicationRoleManager roleManager;

        public UserManageController()
        {
            userManager = new ApplicationUserManager(new ApplicationUserStore(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
        }

        public ActionResult Index()
        {
            var model = userManager.Users
                .Select(p => new UserRoleViewModel
                {
                    Id = p.Id,
                    UserName = p.UserName,
                    DisplayName = p.DisplayName,
                })
                .ToList();

            var helper = new UserManageHelper(userManager, roleManager);

            model.ForEach(p => p.Roles = helper.RoleDictionary(p.Id));

            ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();

            return View(model);
        }

        public ActionResult ChangeUserRole(string id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if (userManager.FindById(id) == null)
            {
                return HttpNotFound();
            }

            var model = userManager.Users
                .Where(p => p.Id == id)
                .Select(p => new UserRoleViewModel
                {
                    Id = p.Id,
                    UserName = p.UserName,
                    DisplayName = p.DisplayName
                })
                .FirstOrDefault();

            var help = new UserManageHelper(userManager, roleManager);
            model.Roles = help.RoleDictionary(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeUserRole(UserRoleViewModel model)
        {
            var user = userManager.FindById(model.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            userManager.RemoveFromRoles(
                model.Id, 
                userManager.GetRoles(model.Id).ToArray()
                );

            userManager.AddToRoles(
                model.Id,
                model.Roles.Where(r => r.Value).Select(r => r.Key).ToArray()
                );

            if (User.Identity.GetUserId() == model.Id)
            {
                var singnInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                singnInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
            }
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}