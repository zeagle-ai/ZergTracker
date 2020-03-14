using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;
using ZergTracker.Models;
using ZergTracker.Models.ViewModels;

namespace ZergTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            if (User.IsInRole("Developer"))
            {

                var devUserId = User.Identity.GetUserId();

                return View(db.Tickets.Where(u => u.AssignedToUserId == devUserId).ToList());
            }
            else if (User.IsInRole("Submitter"))
            {

                var subUserId = User.Identity.GetUserId();

                return View(db.Tickets.Where(u => u.OwnerUserId == subUserId).ToList());
            }


            return View(db.Tickets.ToList()); ;
        }

        //POST Assign a Dev to Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDev(string assignedDev, string UnassignedTickets)
        {

            var ticket = db.Tickets.Find(Int32.Parse(UnassignedTickets));
            
            ticket.AssignedToUserId = assignedDev;
            ticket.AssignedToUser = db.Users.Find(assignedDev);
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
            db.SaveChanges();

            var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
            NotifManager.ManageTicketNotifs(oldTicket, newTicket);

            var redirectUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);

            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);

                msg.Body = "You have been assigned a new Ticket. " +
                    "Please click the following link to view the details  <a href=\"" + redirectUrl + "\">New Ticket</a>";

                msg.Destination = user.Email;
                msg.Subject = "You have been assigned a new Ticket";

                await ems.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                await Task.FromResult(0);
            }

            return RedirectToAction("Index", "Tickets");
        }


        public async Task<ActionResult> EmailTicket(Ticket ticket)
        {
            var redirectUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);

            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);

                msg.Body = "You have been assigned a new Ticket. " +
                    "Please click the following link to view the details  <a href=\"" + redirectUrl + "\">New Ticket</a>";

                msg.Destination = user.Email;
                msg.Subject = "You have been assigned a new Ticket";

                await ems.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                await Task.FromResult(0);
            }

            return RedirectToAction("Index", "Tickets");
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            TicketViewModel model = new TicketViewModel();

            model.Id = ticket.Id;
            model.OwnerUser = ticket.OwnerUser;
            model.ProjectName = ticket.ProjectName;
            model.TicketType = ticket.TicketType;
            model.TicketStatus = ticket.TicketStatus;
            model.TicketPriority = ticket.TicketPriority;
            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.AssignedToUserId = ticket.AssignedToUserId;
            model.AssignedToUser = ticket.AssignedToUser;
            model.Created = ticket.Created;
            model.Updated = ticket.Updated;
            model.TicketComments = ticket.TicketComments;
            model.TicketAttachments = ticket.TicketAttachments;
            model.TicketHistories = ticket.TicketHistories;

            return View(model);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter, Admin")]
        public ActionResult Create(int ProjectId)
        {
            TicketViewModel model = new TicketViewModel();

            model.ProjectId = ProjectId;
            model.ProjectName = db.Projects.SingleOrDefault(n => n.Id == ProjectId)?.Name;
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket, int ProjectId)
        {
            if (ModelState.IsValid)
            {   
                var timeIs = DateTimeOffset.Now;
                ticket.Created = timeIs;
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                ticket.OwnerUserId = userId;
                ticket.OwnerUser = user;
                ticket.ProjectId = ProjectId;
                ticket.ProjectName = db.Projects.SingleOrDefault(n => n.Id == ProjectId)?.Name;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            EditTicketViewModel model = new EditTicketViewModel();

            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.TicketPriorityId = ticket.TicketPriorityId;
            model.TicketTypeId = ticket.TicketTypeId;
            model.TicketStatusId = ticket.TicketStatusId;
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTicketViewModel model)
        {
            var ticket = db.Tickets.Find(model.Id);

            if (ModelState.IsValid)
            {
                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Updated = DateTimeOffset.Now;
                ticket.TicketTypeId = model.TicketTypeId;
                ticket.TicketPriorityId = model.TicketPriorityId;
                ticket.TicketStatusId = model.TicketStatusId;
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                db.SaveChanges();

                HistoryHelper helper = new HistoryHelper();
                var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var userId = User.Identity.GetUserId();
                helper.UpdateHistory(oldTicket, newTicket, userId);
                NotifManager.ManageTicketNotifs(oldTicket, newTicket);
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id});
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(model);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
