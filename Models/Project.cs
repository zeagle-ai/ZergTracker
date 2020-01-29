using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProjectManagerId { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}