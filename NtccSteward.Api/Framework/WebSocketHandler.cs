using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Api.Framework
{
    public class WebSocketHandler : Microsoft.Web.WebSockets.WebSocketHandler
    {
        private static WebSocketCollection _chatClients = new WebSocketCollection();
        private string _username;

        public WebSocketHandler()
        {      }

        public override void OnOpen()
        {
            _chatClients.Add(this);
        }

        public override void OnMessage(string message)
        {
            _chatClients.Broadcast(message);
        }

        public override void OnClose()
        {
            _chatClients.Remove(this);
        }
    }
}