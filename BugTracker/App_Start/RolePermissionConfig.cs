using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker
{
    public static class RolePermissionConfig
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
            "Assign Own Projects"
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
                "Assign All Projects"
            },
            ["Project Manager"] = new List<string>
            {
                "View All Projects",
                "Create Projects",
                "Edit All Projects",
                "Archive All Projects",
                "Assign All Projects"
            },
            ["Developer"] = new List<string>
            {
                "View Own Projects"
            },
            ["Submitter"] = new List<string>
            {
                "View Own Projects"
            }
        };
    }
}