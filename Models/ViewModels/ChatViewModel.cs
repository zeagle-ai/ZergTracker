using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZergTracker.Models.ViewModels
{
    public class ChatViewModel
    {
        public string UserFullName { get; set; }
        public string RecipFullName { get; set; }
        public DateTime Created { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationUser Recip { get; set; }
    }
}