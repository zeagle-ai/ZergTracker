using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZergTracker.Models;

namespace ZergTracker.Helper
{
    public static class NotifManager
    {
        public static void ManageTicketNotifs(Ticket oldTicket, Ticket newTicket)
        {
            ManageAssignmentNotifs(oldTicket, newTicket);
            ManagePropertyChangeNotifs(oldTicket, newTicket);
        }

        private static void ManageAssignmentNotifs(Ticket oldTicket, Ticket newTicket)
        {
            var assigned = oldTicket.AssignedToUserId == null && newTicket.AssignedToUserId != null;
            var unassigned = oldTicket.AssignedToUserId != null && newTicket.AssignedToUserId == null;
            var reassigned = newTicket.AssignedToUserId != null && newTicket.AssignedToUserId != oldTicket.AssignedToUserId;

            TicketNotification notif = new TicketNotification();
            notif.TicketId = oldTicket.Id;
            //Check if assigned/unass/reass
            if (assigned)
            {
                notif.RecipientUserId = newTicket.AssignedToUserId;
                notif.NotifBody = $"Assigned: {newTicket.Title}";
                notif.NotifType = "Urgent";
                notif.Created = DateTime.Now;
                GenerateNotif(notif);
                
            }
            else if (unassigned)
            {
                notif.RecipientUserId = oldTicket.AssignedToUserId;
                notif.NotifBody = $"Unassigned: {newTicket.Title}";
                notif.NotifType = "Low";
                notif.Created = DateTime.Now;
                GenerateNotif(notif);
            }
            else if (reassigned)
            {
                notif.RecipientUserId = oldTicket.AssignedToUserId;
                notif.NotifBody = $"You have been unassigned to Ticket ID: {oldTicket.Id}, titled: {oldTicket.Title}";
                notif.NotifType = "Low";
                notif.Created = DateTime.Now;
                GenerateNotif(notif);
                notif.RecipientUserId = newTicket.AssignedToUserId;
                notif.NotifBody = $"You have been assigned to Ticket ID: {newTicket.Id}, titled: {newTicket.Title}";
                notif.NotifType = "Urgent";
                notif.Created = DateTime.Now;
                GenerateNotif(notif);
            }
        }
        private static void ManagePropertyChangeNotifs(Ticket oldTicket, Ticket newTicket)
        {
            TicketNotification notif = new TicketNotification();
            notif.TicketId = oldTicket.Id;
            //send properties
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                notif.Created = DateTime.Now;
                notif.NotifType = "Urgent";
                notif.RecipientUserId = oldTicket.AssignedToUserId;
                notif.NotifBody = $"The Priority of {newTicket.Title} has been changed to {newTicket.TicketPriority.Name}, Ticket ID:  {newTicket.Id}";
                GenerateNotif(notif);
            }
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                notif.Created = DateTime.Now;
                notif.NotifType = "Urgent";
                notif.RecipientUserId = oldTicket.AssignedToUserId;
                notif.NotifBody = $"The Status of {newTicket.Title} has been changed to {newTicket.TicketStatus.Name}, Ticket ID:  {newTicket.Id}";
                GenerateNotif(notif);
            }
        }
        public static void ManageCommentNotifs(TicketComment comment)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            TicketNotification notif = new TicketNotification();

            if (comment.Comment != null)
            {
                var ticket = db.Tickets.Find(comment.TicketId);
                if (ticket.AssignedToUserId != null)
                {
                    notif.RecipientUserId = ticket.AssignedToUserId;
                }
                else
                {
                    notif.RecipientUserId = ticket.OwnerUserId;
                }
                notif.Created = DateTime.Now;
                notif.NotifType = "Low";
                if (comment.HasPic)
                {
                    notif.NotifBody = $"Comments and Attachments have been added to {ticket.Title} with ID: {ticket.Id}";
                    GenerateNotif(notif);
                }
                else
                {
                    notif.NotifBody = $"Comments have been added to {ticket.Title} with ID: {ticket.Id}";
                    GenerateNotif(notif);
                }
            }
        }
        private static void GenerateNotif(TicketNotification notif)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            notif.HasBeenRead = false;
            db.TicketNotifications.Add(notif);
            db.SaveChanges();
        }
    }
}