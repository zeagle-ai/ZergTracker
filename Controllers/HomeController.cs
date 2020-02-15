using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;
using ZergTracker.Models;
using ZergTracker.Models.ViewModels;

namespace ZergTracker.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();

            model.ProjectCount = db.Projects.Count();
            int inDevTick = db.Tickets.Where(s => s.TicketStatus.Name == "In Development").Count();
            int newTick = db.Tickets.Where(s => s.TicketStatus.Name == "New").Count();
            model.TicketCount = (inDevTick + newTick);
            model.UserCount = db.Users.Count();
            int urg = db.Tickets.Where(p => p.TicketPriority.Name == "Urgent").Count();
            model.UrgentTicketCount = urg;
            model.MedTickCount = db.Tickets.Where(p => p.TicketPriority.Name == "Medium").Count();
            model.LowTickCount = db.Tickets.Where(p => p.TicketPriority.Name == "Low").Count();
            model.HighTickCount = db.Tickets.Where(p => p.TicketPriority.Name == "High").Count();

            int devUsers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "c4e1a39e-19a2-45b3-80be-46f9c2fa45ca")).Count();
            if (model.TicketCount != 0 && devUsers != 0)
            {
                int tickPerDev = model.TicketCount / devUsers;
                model.TicketsPerDev = tickPerDev;
                model.TicketsPerDevStatusBar = (tickPerDev * 10) + "%";
            }
            else
            {
                model.TicketsPerDev = 0;
                model.TicketsPerDevStatusBar = "0%";
            }

            int pmUsers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "f462585d-af2b-4bfb-bed8-7aa5789956e4")).Count();

            if (model.ProjectCount != 0 && pmUsers != 0)
            {
                int projPerPM = model.ProjectCount / pmUsers;
                model.ProjPerPM = projPerPM;
                model.ProjPerPMStatusBar = (projPerPM * 20) + "%";
            }
            else
            {
                model.ProjPerPM = 0;
                model.ProjPerPMStatusBar = "0%";
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();

            return View(model);
        }

        // Get Profile info in sidebar
        [AllowAnonymous]
        public ActionResult UserProfileInfo()
        {
            ApplicationUser user = new ApplicationUser();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                user = db.Users.FirstOrDefault(u => u.Id == userId);
            }
            else
            {
                user.FirstName = "Anonymous";
                user.LastName = "User";
                user.ProfilePic = "/assets/img/defaults/default-profile-pic-1.png";
            }
            return View(user);
        }

        //Get Comment Profile Pic
        public ActionResult CommentProfilePic()
        {
            ApplicationUser user = new ApplicationUser();
            var userId = User.Identity.GetUserId();
            user = db.Users.FirstOrDefault(u => u.Id == userId);

            return View(user);
        }

        [AllowAnonymous] // Change this to only submitters
        public ActionResult NavRoleItems()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.ProjectId = new SelectList(db.Projects.Where(u => u.Users.Any(i => i.Id == userId)), "Id", "Name");
            ViewBag.AssignedDev = new SelectList(db.Users.Where(r => r.Roles.Any(d => d.RoleId == "c4e1a39e-19a2-45b3-80be-46f9c2fa45ca")), "Id", "FirstName");
            ViewBag.UnassignedTickets = new SelectList(db.Tickets.Where(a => a.AssignedToUserId == null), "Id", "Title");

            return View();
        }

        [HttpGet]
        public ActionResult GetTicketsByProject(string devs)
        {
            if (!string.IsNullOrWhiteSpace(devs))
            {
                UserProjectsHelper helper = new UserProjectsHelper();

                IEnumerable<SelectListItem> ticketsByProj = helper.EnumerateUserProjectTickets(devs);
                return Json(ticketsByProj, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        //GET Notifications
        public ActionResult Notifications()
        {
            NotificationsViewModel notification = new NotificationsViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                notification.Notifs = db.TicketNotifications.Where(t => t.RecipientUserId == userId).Where(r => r.HasBeenRead == false).ToList();
            }

            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notifications(int id, int ticketId)
        {
            var notif = db.TicketNotifications.Find(id);
            notif.HasBeenRead = true;
            var ticket = db.Tickets.Find(ticketId);
            ticket.Updated = DateTimeOffset.Now;
            var inDev = db.TicketStatuses.FirstOrDefault(i => i.Name == "In Development").Id;
            ticket.TicketStatusId = inDev;

            db.SaveChanges();

            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: {0}" + "({1}) </p> <p>Message: </p> <p>{2}</p>";

                    var svc = new EmailService();
                    var msg = new IdentityMessage()
                    {
                        Subject = "Contact from Zerg Tracker",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        Destination = "laymanscode@gmail.com"
                    };

                    await svc.SendAsync(msg);
                    return View();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}