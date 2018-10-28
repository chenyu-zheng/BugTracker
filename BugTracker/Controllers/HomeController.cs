using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using BugTracker.Helpers;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            var model = new DashboardViewModel
            {
                NumberOfMyProjects = user.Projects.Count(),
                NumberOfProjectsTickets = user.Projects.SelectMany(p => p.Tickets).Count(),
                NumberOfAssignedTickets = user.AssignedTickets.Count(),
                NumberOfCreatedTickets = user.Tickets.Count()
            };
            
            var uHelper = new UserManageHelper(db);
            if (uHelper.HasPermission(userId, "Receive Tickets"))
            {
                var ticketUpdates = db.TicketRevisions
                    .Where(r => r.Ticket.AssigneeId == userId &&
                    !(r.Details.Any(d => d.Property == "AssigneeId")))
                    .OrderByDescending(r => r.Created)
                    .Take(5)
                    .ProjectTo<TicketUpdateViewModel>(MappingConfig.Config)
                    .ToList();
                    var vMHelper = new ViewModelHelper(db);
                model.TicketUpdates = vMHelper.ReformTicketRevisions(
                        ticketUpdates.Cast<TicketRevisionViewModel>().ToList())
                        .Cast<TicketUpdateViewModel>().ToList();

                model.AssignedTickets = db.Tickets
                    .Where(t => t.AssigneeId == userId &&
                        t.Status.Name == "Assigned")
                    .OrderByDescending(t => t.Updated.HasValue ? t.Updated : t.Created)
                    .Take(10)
                    .ProjectTo<TicketViewModel>(MappingConfig.Config)
                    .ToList();
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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