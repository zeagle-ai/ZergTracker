using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;
using ZergTracker.Models;
using ZergTracker.Models.ViewModels;

namespace ZergTracker.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private UserProjectsHelper projHelper = new UserProjectsHelper();

        // GET: Roles
        public ActionResult Personnel()
        {
            PersonnelViewModel model = new PersonnelViewModel();
            model.UserId = new SelectList(db.Users, "Id", "FirstName");
            model.RoleName = new MultiSelectList(db.Roles, "Name", "Name");
            model.ProjectId = new SelectList(db.Projects, "Id", "Name");
            model.PMers = new SelectList(db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")), "Id", "FirstName");
            var userTitles = new List<ApplicationUserTitles>();
            model.UserTitles = userTitles;

            var userList = db.Users.ToList();

            foreach (var user in userList)
            {
                ApplicationUserTitles personnel = new ApplicationUserTitles();
                personnel.FirstName = user.FirstName;
                personnel.LastName = user.LastName;
                personnel.RoleNameStrings = rolesHelper.ListUserRoles(user.Id);
                personnel.ProjectNameStrings = projHelper.ListUserProjects(user.Id);

                userTitles.Add(personnel);
            }

            return View(model);
        }

        // POST: Assign Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult ChangeUserRole(string userId, List<string> roleName, bool add)
        {
            if (add)
            {
                foreach (string role in roleName)
                {
                    if (!rolesHelper.IsUserInRole(userId, role))
                    {
                        rolesHelper.AddUserToRole(userId, role);
                    }
                }
            }
            else 
            {
                foreach (var role in roleName)
                {
                    if (rolesHelper.IsUserInRole(userId, role))
                    {
                        rolesHelper.RemoveUserFromRole(userId, role);
                    }
                }
            }
                return RedirectToAction("Personnel");
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