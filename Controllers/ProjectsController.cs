using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;
using ZergTracker.Models;
using ZergTracker.Models.ViewModels;

namespace ZergTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserProjectsHelper helper = new UserProjectsHelper();

        // GET: Projects/Index
        public ActionResult Index()
        {

            ProjectViewModel model = new ProjectViewModel();

            if (User.IsInRole("Admin"))
            {
                model.Projects = db.Projects.ToList();
                model.ProjectManager = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).ToList();
                return View(model);
            }
            else if (User.IsInRole("ProjectManager"))
            {
                var userId = User.Identity.GetUserId();

                model.Projects = db.Projects.Where(p => p.ProjectManagerId == userId).ToList();
                model.ProjectManager = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).ToList();
                return View(model);
            }
            else if (User.IsInRole("Developer") || User.IsInRole("Submitter"))
            { 
                var userId = User.Identity.GetUserId();

                var user = db.Users.Find(userId);

                model.Projects = user.Projects.ToList();
                model.ProjectManager = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).ToList();
                return View(model);
            }

            model.ProjectManager = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).ToList();
            return View(model);
        }

        // Get All Projects
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AllProjects()
        {

            return View(db.Projects.ToList());
            
        }

        public ActionResult Personnel(PersonnelViewModel model)
        {
            model.UserId = new SelectList(db.Users, "Id", "FirstName");
            model.ProjectId = new SelectList(db.Projects, "Id", "Name");

            return View(model);
        }

        // POST Change User Projects
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult ChangeUserProjects(string userId, int projectId, bool add)
        {
            //i need another check to go to badrequest
            if (add)
            {
                helper.AddUserToProject(userId, projectId);
            }
            else
            {
                helper.RemoveUserFromProject(userId, projectId);
            }
            return RedirectToAction("Personnel", "Roles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult ChangeProjectPM(string pmers, int projectId, bool add)
        {
            //i need another check to go to badrequest
            if (add)
            {
                helper.AddPM(pmers, projectId);
            }
            else
            {
                helper.RemovePM(pmers, projectId);
            }
            return RedirectToAction("Personnel", "Roles");
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            ProjectViewModel model = new ProjectViewModel();
            model.Users = db.Users.Where(p => p.Projects.Any(i => i.Id == id)).ToList();
            model.Tickets = db.Tickets.Where(p => p.ProjectId == id).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            model.Name = project.Name;
            model.Decription = project.Decription;
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.ProjectManagerId != null)
            {
                model.ProjectManager = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).ToList();
            }
            return View(model);
        }

        // GET: Projects/Create
        [Authorize(Roles ="Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Name,Decription")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
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
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit([Bind(Include = "Id,Name,ProjectManagerId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
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
