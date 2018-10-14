using BugTracker.Models;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Helpers
{
    public class ViewModelHelper
    {
        private ApplicationDbContext db;

        public ViewModelHelper()
        {
            db = new ApplicationDbContext();

        }

        public CreateTicketViewModel AddSelectLists(CreateTicketViewModel viewModel, string userId)
        {
            viewModel.ProjectList = new SelectList(db.Projects
                    .Where(p => p.Members.Any(m => m.Id == userId))
                    .Select(p => new { p.Name, p.Id }),
                    "Id", "Name");
            viewModel.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
            viewModel.CategoryList = new SelectList(db.TicketCategories, "Id", "Name");
            return viewModel;
        }

        public EditTicketViewModel AddSelectLists(EditTicketViewModel viewModel)
        {
            var status = db.TicketStatus.ToList();
            if (status.FirstOrDefault(s => s.Id == viewModel.StatusId).Name == "New")
            {
                status.RemoveAll(s => s.Name == "Assigned");
            }
            else
            {
                status.RemoveAll(s => s.Name == "New");
            }
            viewModel.ProjectList = new SelectList(db.Projects.Where(p => p.Id == viewModel.ProjectId), "Id", "Name");
            viewModel.CategoryList = new SelectList(db.TicketCategories, "Id", "Name");
            viewModel.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
            viewModel.StatusList = new SelectList(status, "Id", "Name");
            return viewModel;
        }
    }
}