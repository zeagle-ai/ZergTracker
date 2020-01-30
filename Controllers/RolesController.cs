using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;

namespace ZergTracker.Controllers
{
    public class RolesController : Controller
    {
        private UserRolesHelper helper = new UserRolesHelper();

        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        // POST: Roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUserRole(string userId, string roleId)
        {
            if (helper.AddUserToRole(userId, roleId))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }        
        }
    }
}