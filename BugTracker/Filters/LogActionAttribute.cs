using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ActionFilters
{
    public class LogActionAttribute : ActionFilterAttribute
    {
        private Stopwatch stopwatch = new Stopwatch();
        private ApplicationDbContext db = new ApplicationDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stopwatch.Restart();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            stopwatch.Stop();
            var log = new ActionLog
            {
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                ActionName = filterContext.ActionDescriptor.ActionName,
                ExecutionTimeMS = stopwatch.ElapsedMilliseconds
            };
            db.ActionLogs.Add(log);
            var count = db.ActionLogs.Count();
            if (count > 300)
            {
                db.ActionLogs.RemoveRange(db.ActionLogs.Take(count - 200));
            }
            db.SaveChanges();
        }
    }
}