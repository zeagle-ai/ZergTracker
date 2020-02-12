using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public int TicketId { get; set; }
        public string RecipientUserId { get; set; }
        public string NotifType { get; set; }
        public string NotifBody { get; set; }
        public string NewUserId { get; set; }
        public bool HasBeenRead { get; set; }
        public DateTime Created { get; set; }
        public int NotifCount { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}