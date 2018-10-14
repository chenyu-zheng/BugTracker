﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker
{
    public static class AppDataConfig
    {
        public static IReadOnlyList<string> Roles = new List<string>
        {
            "Admin",
            "Project Manager",
            "Developer",
            "Submitter"
        };

        public static IReadOnlyList<string> Permissions = new List<string>
        {
            "Edit User Roles",
            "View All Projects",
            "View Own Projects",
            "Create Projects",
            "Edit All Projects",
            "Edit Own Projects",
            "Archive All Projects",
            "Archive Own Projects",
            "Assign All Projects",
            "Assign Own Projects",
            "Assign Tickets",
            "Create Tickets",
            "Edit Tickets"
        };

        public static IReadOnlyDictionary<string, IReadOnlyList<string>> RolePermissions = new Dictionary<string, IReadOnlyList<string>>
        {
            ["Admin"] = new List<string>
            {
                "Edit User Roles",
                "View All Projects",
                "Create Projects",
                "Edit All Projects",
                "Archive All Projects",
                "Assign All Projects",
                "Edit Tickets",
                "Assign Tickets"
            },
            ["Project Manager"] = new List<string>
            {
                "View All Projects",
                "Create Projects",
                "Edit All Projects",
                "Archive All Projects",
                "Assign All Projects",
                "Edit Tickets",
                "Assign Tickets"
            },
            ["Developer"] = new List<string>
            {
                "View Own Projects",
                "Edit Tickets"
            },
            ["Submitter"] = new List<string>
            {
                "View Own Projects",
                "Create Tickets",
                "Edit Tickets"
            }
        };

        public static IReadOnlyList<string> TicketCategories = new List<string>
        {
            "Bug",
            "Feature",
            "Support"
        };

        public static IReadOnlyList<string> TicketStatus = new List<string>
        {
            "New",
            "Assigned",
            "Resolved",
            "Feedback",
            "Closed",
            "Rejected"
        };

        public static IReadOnlyList<string> TicketPriorities = new List<string>
        {
            "Low",
            "Normal",
            "High",
            "Urgent"
        };


    }
}