using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ActionFilters
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private string _permissions;

        public PermissionAuthorizeAttribute(string permissions)
        {
            _permissions = permissions;
        }

        public override void OnAuthorization(AuthorizationContext context)
        {
            //var controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            //var actionName = context.ActionDescriptor.ActionName;
            //context.HttpContext.Request["id"]

            if (!AuthorizeCore(context.HttpContext))
            {
                context.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Login"
                        })
                    );
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var db = new ApplicationDbContext();
            var allowedRoles = db.Roles
                .Where(r => r.Permissions
                    .Any(p => _permissions.Contains(p.Name))
                )
                .Select(r => r.Name);

            Roles = string.Join(",", allowedRoles);

            return base.AuthorizeCore(httpContext);
        }

    }
}