using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace NtccSteward.Api.Controllers
{
    public class SocketController : ApiController
    {
        public async Task<IHttpActionResult> Get(int churchId)
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessWebsocketSession);
            }

            return await Task.FromResult(ResponseMessage(Request.CreateResponse(HttpStatusCode.SwitchingProtocols)));
        }       

        private Task ProcessWebsocketSession(AspNetWebSocketContext context)
        {
            var handler = new Framework.WebSocketHandler();
            var processTask = handler.ProcessWebSocketRequestAsync(context);
            return processTask;
        }
    }
}
