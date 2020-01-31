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
        private PersonnelViewModel model = new PersonnelViewModel();

        // GET: Roles
        public ActionResult Personnel()
        {
            model.UserId = new SelectList(db.Users, "Id", "FirstName");
            model.RoleName = new SelectList(db.Roles, "Name", "Name");
            model.UserRoles = db.Users.ToList();

            return View(model);
        }

        // POST: Assign Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserRole(string userId, string roleId, bool add)
        {
            if (!helper.IsUserInRole(userId, roleId) && add)
            {
                helper.AddUserToRole(userId, roleId);
                return RedirectToAction("Personnel");
            }
            else if (helper.IsUserInRole(userId, roleId) && !add)
            {
                helper.RemoveUserFromRole(userId, roleId);
                return RedirectToAction("Personnel");
            } 
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }      
        }
    }
}