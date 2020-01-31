using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZergTracker.Models.ViewModels
{
    public class PersonnelViewModel
    {
        public SelectList UserId { get; set; }
        public SelectList RoleName { get; set; }
        public SelectList ProjectName { get; set; }
        public ICollection<ApplicationUser> UserRoles { get; set; }
        public ICollection<Project> UserProjects { get; set; }
    }
}