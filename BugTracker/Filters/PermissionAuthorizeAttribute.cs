using BugTracker.Helpers;
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
        private List<string> _permissions;

        public PermissionAuthorizeAttribute(string permissions)
        {
            _permissions = permissions.Split(',').Select(p => p.Trim()).ToList();
        }

        public override void OnAuthorization(AuthorizationContext context)
        {

            //var param = context.RouteData.Values["id"];

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
            var helper = new UserManageHelper();

            var userId = httpContext.User.Identity.GetUserId();

            foreach(var p in _permissions)
            {
                if (helper.HasPermission(userId, p))
                {
                    return true;
                }
            }
            return false;
        }

    }
}