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
        private UserRolesHelper helper = new UserRolesHelper();

        // GET: Roles
        public ActionResult Personnel(PersonnelViewModel model)
        {
            model.UserId = new SelectList(db.Users, "Id", "FirstName");
            model.RoleName = new MultiSelectList(db.Roles, "Name", "Name");
            model.UserRoles = db.Users.ToList();

            return View(model);
        }

        // POST: Assign Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserRole(string userId, List<string> roleName, bool add)
        {
            if (add)
            {
                foreach (string role in roleName)
                {
                    if (!helper.IsUserInRole(userId, role))
                    {
                        helper.AddUserToRole(userId, role);
                    }
                }
            }
            else 
            {
                foreach (var role in roleName)
                {
                    if (helper.IsUserInRole(userId, role))
                    {
                        helper.RemoveUserFromRole(userId, role);
                    }
                }
            }
                return RedirectToAction("Personnel");
        }
    }
}