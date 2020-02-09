using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZergTracker.Helper;

namespace ZergTracker.Models.ViewModels
{
    public class PersonnelViewModel
    {
        public SelectList UserId { get; set; }
        public MultiSelectList RoleName { get; set; }
        public SelectList ProjectId { get; set; }
        public SelectList PMers { get; set; }
        public ICollection<ApplicationUserTitles> UserTitles { get; set; }
    }
     public class ApplicationUserTitles : ApplicationUser
    {
        public ICollection<string> RoleNameStrings { get; set; }
        public ICollection<Project> ProjectNameStrings { get; set; }
    }
}