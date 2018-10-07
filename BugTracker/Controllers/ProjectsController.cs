using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogApp.Helpers;
using BugTracker.ActionFilters;
using BugTracker.Models;
using BugTracker.ViewModels;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            var model = db.Projects
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Identifier = p.Identifier,
                    Created = p.Created,
                    Updated = p.Updated,
                    NumberOfMembers = p.Members.Count(),
                    NumberOfTickets = p.Tickets.Count()
                })
                .ToList();
            return View(model);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = db.Projects
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Identifier = p.Identifier,
                    Created = p.Created,
                    Updated = p.Updated,
                    NumberOfMembers = p.Members.Count(),
                    NumberOfTickets = p.Tickets.Count()
                })
                .FirstOrDefault(p => p.Id == id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Projects/Create
        [PermissionAuthorize("Create Projects")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("Create Projects")]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                string identifier = StringUtilities.URLFriendly(project.Name);
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    ModelState.AddModelError(nameof(project.Name), "Invalid project name");
                    return View(project);
                }
                if (db.Projects.Any(p => p.Identifier == identifier))
                {
                    ModelState.AddModelError(nameof(project.Name), "Project name must be unique");
                    return View(project);
                }
                project.Identifier = identifier;

                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [PermissionAuthorize("Edit Projects")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                Project op = db.Projects.Where(p => p.Id == project.Id).FirstOrDefault();
                if (op == null)
                {
                    return HttpNotFound();
                }

                if (project.Name != op.Name)
                {
                    string identifier = StringUtilities.URLFriendly(project.Name);
                    if (string.IsNullOrWhiteSpace(identifier))
                    {
                        ModelState.AddModelError(nameof(project.Name), "Invalid project name");
                        return View(project);
                    }
                    if (db.Projects.Any(p => p.Identifier == identifier))
                    {
                        ModelState.AddModelError(nameof(project.Name), "Project name must be unique");
                        return View(project);
                    }
                    op.Identifier = identifier;
                }
                op.Name = project.Name;
                op.Description = project.Description;
                op.Updated = DateTime.Now;

                db.Entry(op).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = db.Projects
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Identifier = p.Identifier,
                    Created = p.Created,
                    Updated = p.Updated,
                    NumberOfMembers = p.Members.Count(),
                    NumberOfTickets = p.Tickets.Count()
                })
                .FirstOrDefault(p => p.Id == id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeMember(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = db.Projects
                .Where(p => p.Id == id)
                .Include(p => p.Members)
                .Select(p => new ChangeMemberViewModel
                {
                    Project = new ProjectViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Identifier = p.Identifier,
                        Created = p.Created,
                        Updated = p.Updated,
                        NumberOfMembers = p.Members.Count(),
                    },

                    Members = p.Members.Select(m => new UserViewModel
                    {
                        Id = m.Id,
                        UserName = m.UserName,
                        DisplayName = m.DisplayName
                    }).ToList(),

                    Users = db.Users
                    .Where(u => !p.Members.Any(m => m.Id == u.Id))
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        DisplayName = u.DisplayName
                    }).ToList()
                })
                .FirstOrDefault();

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        public ActionResult AddMember(int projectId, string userId)
        {
            var project = db.Projects.
                Include(p => p.Members)
                .FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.Members.Any(m => m.Id == userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            project.Members.Add(user);
            db.SaveChanges();

            return RedirectToAction("ChangeMember", new { id = projectId });
        }

        public ActionResult RemoveMember(int projectId, string userId)
        {
            var project = db.Projects.
                Include(p => p.Members)
                .FirstOrDefault(p => p.Id == projectId);
            var user = project.Members.FirstOrDefault(m => m.Id == userId);

            if (project == null || user == null)
            {
                return HttpNotFound();
            }

            project.Members.Remove(user);
            db.SaveChanges();

            return RedirectToAction("ChangeMember", new { id = projectId });
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
