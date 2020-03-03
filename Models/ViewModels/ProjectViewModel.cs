using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models.ViewModels
{
    public class ProjectViewModel
    {
        public string Name { get; set; }
        public string ProjectManagerId { get; set; }
        public ICollection<ApplicationUser> ProjectManager { get; set; }
        public string Decription { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}