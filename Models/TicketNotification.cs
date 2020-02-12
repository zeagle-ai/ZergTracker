using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string RecipientUserId { get; set; }
        public string NotifType { get; set; }
        public string NotifBody { get; set; }
        public string NewUserId { get; set; }
        public bool HasBeenRead { get; set; }
        public DateTime Created { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser RecipientUser { get; set; }
        public virtual ApplicationUser NewUser { get; set; }
    }
}