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
        public ActionResult AssignDev(string assignedDev, int unassignedTickets)
        {

            var ticket = db.Tickets.Find(unassignedTickets);
            
            ticket.AssignedToUserId = assignedDev;
            ticket.AssignedToUser = db.Users.Find(assignedDev);
            db.SaveChanges();

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
            model.AssignedToUser = ticket.AssignedToUser;
            model.Created = ticket.Created;
            model.Updated = ticket.Updated;
            model.TicketComments = ticket.TicketComments;
            model.TicketAttachments = ticket.TicketAttachments;

            return View(model);
        }

        // GET: Tickets/Create
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
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                HistoryHelper helper = new HistoryHelper();
                var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var userId = User.Identity.GetUserId();
                helper.UpdateHistory(oldTicket, newTicket, userId);
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id});
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
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
