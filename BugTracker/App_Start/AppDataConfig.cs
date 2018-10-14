using System;
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
            "Assign All Projects",
            "Assign Own Projects",
            "Delete Projects",
            "Assign Tickets",
            "Create Tickets",
            "List All Tickets",
            "List Projects Tickets",
            "List Assigned Tickets",
            "List Created Tickets",
            "Edit All Tickets",
            "Edit Projects Tickets",
            "Edit Assigned Tickets",
            "Edit Created Tickets",
            "Edit Ticket Status",
            "Receive Tickets",
            "Delete Tickets",
        };

        public static IReadOnlyDictionary<string, IReadOnlyList<string>> RolePermissions = new Dictionary<string, IReadOnlyList<string>>
        {
            ["Admin"] = new List<string>
            {
                "Edit User Roles",
                "View All Projects",
                "Create Projects",
                "Edit All Projects",
                "Assign All Projects",
                "Delete Projects",
                "Assign Tickets",
                "List All Tickets",
                "Edit All Tickets",
                "Edit Ticket Status",
                "Delete Tickets",
            },
            ["Project Manager"] = new List<string>
            {
                "View All Projects",
                "Create Projects",
                "Edit All Projects",
                "Assign All Projects",
                "Assign Tickets",
                "List All Tickets",
                "List Projects Tickets",
                "Edit Projects Tickets",
                "Edit Ticket Status",
            },
            ["Developer"] = new List<string>
            {
                "View Own Projects",
                "List All Tickets",
                "List Projects Tickets",
                "List Assigned Tickets",
                "Edit Assigned Tickets",
                "Receive Tickets",
            },
            ["Submitter"] = new List<string>
            {
                "View Own Projects",
                "Create Tickets",
                "List All Tickets",
                "List Created Tickets",
                "Edit Created Tickets",
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