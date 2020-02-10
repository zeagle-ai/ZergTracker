using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Models;
using ZergTracker.Models.ViewModels;

namespace ZergTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            ApplicationDbContext db = new ApplicationDbContext();

            model.ProjectCount = db.Projects.Count();
            model.TicketCount = db.Tickets.Count();
            model.UserCount = db.Users.Count();
            int urg = db.Tickets.Where(p => p.TicketPriority.Name == "Urgent").Count();
            model.UrgentTicketCount = urg;

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