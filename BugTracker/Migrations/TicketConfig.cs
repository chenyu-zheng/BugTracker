using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Migrations
{
    public class TicketConfig
    {
        private ApplicationDbContext db;
        private IEnumerable<string> categories;
        private IEnumerable<string> priorities;
        private IEnumerable<string> status;

        public TicketConfig(
            ApplicationDbContext db,
            IEnumerable<string> categories,
            IEnumerable<string> priorities,
            IEnumerable<string> status
            )
        {
            this.db = db;
            this.categories = categories;
            this.priorities = priorities;
            this.status = status;
        }

        public void Init()
        {
            InitCategories();
            InitPriorities();
            InitStatus();
        }

        public void InitCategories()
        {
            foreach (var item in categories)
            {
                if (!db.TicketCategories.Any(p => p.Name == item))
                {
                    db.TicketCategories.Add(new TicketCategory { Name = item });
                }
            }
            db.SaveChanges();
        }

        public void InitPriorities()
        {
            foreach (var item in priorities)
            {
                if (!db.TicketPriorities.Any(p => p.Name == item))
                {
                    db.TicketPriorities.Add(new TicketPriority { Name = item });
                }
            }
            db.SaveChanges();
        }

        public void InitStatus()
        {
            foreach (var item in status)
            {
                if (!db.TicketStatus.Any(p => p.Name == item))
                {
                    db.TicketStatus.Add(new TicketStatus { Name = item });
                }
            }
            db.SaveChanges();
        }
    }
}