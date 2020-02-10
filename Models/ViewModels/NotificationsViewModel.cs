using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string TicketStatus { get; set; }
        public int NotifCount { get; set; }
        public string NewUserId { get; set; }
        public bool HasBeenRead { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ApplicationUser> UserBase { get; set; }
    }
}