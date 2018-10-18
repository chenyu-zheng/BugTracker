using BugTracker.Models;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public ViewModelHelper(ApplicationDbContext db)
        {
            this.db = db;
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

        public TicketDetailsViewModel ReformTicketRevisions(TicketDetailsViewModel viewModel)
        {
            viewModel.Revisions = viewModel.Revisions.OrderByDescending(r => r.Created).ToList();
            var props = new List<string>
            {
                "CategoryId",
                "StatusId",
                "PriorityId",
                "AssigneeId",
            };
            var details = viewModel.Revisions
                .SelectMany(r => r.Details)
                .Where(d => props.Contains(d.Property));
            if (!details.Any())
            {
                return viewModel;
            }
            foreach (var item in details)
            {
                if (item.Property == "AssigneeId")
                {
                    item.Property = "Assignee";
                    item.OldValue = item.OldValue == null ? 
                        null : db.Users.FirstOrDefault(x => x.Id == item.OldValue).DisplayName;
                    item.NewValue = item.NewValue == null ?
                        null : db.Users.FirstOrDefault(x => x.Id == item.NewValue).DisplayName;
                }
                else
                {
                    int oldId = int.Parse(item.OldValue);
                    int newId = int.Parse(item.NewValue);
                    if (item.Property == "CategoryId")
                    {
                        item.Property = "Category";
                        item.OldValue = db.TicketCategories.FirstOrDefault(x => x.Id == oldId).Name;
                        item.NewValue = db.TicketCategories.FirstOrDefault(x => x.Id == newId).Name;
                    }
                    else if (item.Property == "StatusId")
                    {
                        item.Property = "Status";
                        item.OldValue = db.TicketStatus.FirstOrDefault(x => x.Id == oldId).Name;
                        item.NewValue = db.TicketStatus.FirstOrDefault(x => x.Id == newId).Name;
                    }
                    else if (item.Property == "PriorityId")
                    {
                        item.Property = "Priority";
                        item.OldValue = db.TicketPriorities.FirstOrDefault(x => x.Id == oldId).Name;
                        item.NewValue = db.TicketPriorities.FirstOrDefault(x => x.Id == newId).Name;
                    }
                }
            }
            return viewModel;
        }
    }
}