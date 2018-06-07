using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtccSteward.Api.Controllers
{
    [AllowAnonymous]
    public class InfoController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("NtccSteward.Api Info Controller");
        }
    }
}
