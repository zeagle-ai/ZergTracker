using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZergTracker.Models;

namespace ZergTracker.Helper
{
    public class HistoryHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        TicketHistory history = new TicketHistory();

        public void UpdateHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {

            if (oldTicket.Title != newTicket.Title)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Title";
                history.OldValue = oldTicket.Title;
                history.NewValue = newTicket.Title;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
            if (oldTicket.TicketPriority.Name != newTicket.TicketPriority.Name)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Priority";
                history.OldValue = oldTicket.TicketPriority.Name;
                history.NewValue = newTicket.TicketPriority.Name;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
            if (oldTicket.TicketStatus.Name != newTicket.TicketStatus.Name)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Status";
                history.OldValue = oldTicket.TicketStatus.Name;
                history.NewValue = newTicket.TicketStatus.Name;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
            if (oldTicket.TicketType.Name != newTicket.TicketType.Name)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Type";
                history.OldValue = oldTicket.TicketType.Name;
                history.NewValue = newTicket.TicketType.Name;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
            if (oldTicket.Description != newTicket.Description)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Description";
                history.OldValue = oldTicket.Description;
                history.NewValue = newTicket.Description;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
            if (oldTicket.AssignedToUserId != newTicket.AssignedToUserId)
            {
                history.TicketId = newTicket.Id;
                history.Property = "Developer";
                history.OldValue = oldTicket.AssignedToUserId;
                history.NewValue = newTicket.AssignedToUserId;
                history.Changed = DateTimeOffset.Now;
                history.UserId = userId;
                history.User = db.Users.Find(userId);
                db.TicketHistories.Add(history);
                db.SaveChanges();
            }
        }
    }
}