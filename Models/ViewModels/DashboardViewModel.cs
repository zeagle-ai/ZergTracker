using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ProjectCount { get; set; }
        public int TicketCount { get; set; }
        public int UserCount { get; set; }
        public int UrgentTicketCount { get; set; }
        public int TicketsPerDev { get; set; }
        public string TicketsPerDevStatusBar { get; set; }
    }
}