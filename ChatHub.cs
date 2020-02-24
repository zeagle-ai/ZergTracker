using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ZergTracker
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message, string pic)
        {
            Clients.All.addNewMessageToPage(name, message, pic);
        }
    }
}