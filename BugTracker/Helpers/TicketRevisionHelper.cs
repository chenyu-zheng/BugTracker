﻿using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class TicketRevisionHelper
    {
        private ApplicationDbContext db;
        private HttpContextBase httpContext;

        public TicketRevisionHelper(ApplicationDbContext db)
        {
            this.db = db;
        }

        public TicketRevisionHelper(ApplicationDbContext db, HttpContextBase httpContext)
        {
            this.db = db;
            this.httpContext = httpContext;
        }

        public TicketRevision CreateRevision(Ticket ticket, string userId)
        {
            var entry = db.Entry(ticket);
            var changes = new List<TicketRevisionDetail>();

            foreach (var prop in entry.OriginalValues.PropertyNames)
            {
                var originalValue = entry.OriginalValues[prop]?.ToString();
                var currentValue = entry.CurrentValues[prop]?.ToString();
                if (originalValue != currentValue && prop != "Updated")
                {
                    changes.Add(new TicketRevisionDetail
                    {
                        Property = prop,
                        OldValue = originalValue,
                        NewValue = currentValue,
                    });
                }
            }

            if (changes.Any())
            {
                var revision = new TicketRevision
                {
                    TicketId = ticket.Id,
                    UserId = userId,
                    Created = ticket.Updated.Value,
                    Details = changes
                };
                return revision;
            }
            return null;
        }

        public TicketRevision CreateAttachmentRevision(Attachment attachment, string userId)
        {
            var entry = db.Entry(attachment);
            var revision = new TicketRevision
            {
                TicketId = attachment.TicketId,
                UserId = userId
            };
            var detail = new TicketRevisionDetail
            {
                Property = "Attachment"
            };
            if (entry.State == EntityState.Added)
            {
                detail.NewValue = attachment.FileName;
                revision.Created = attachment.Created;
            }
            else if (entry.State == EntityState.Deleted)
            {
                detail.OldValue = attachment.FileName;
                revision.Created = DateTimeOffset.Now;
            }
            else
            {
                return null;
            }
            revision.Details.Add(detail);
            return revision;
        }
    }
}