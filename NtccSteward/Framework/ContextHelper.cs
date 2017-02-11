using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using NtccSteward.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Framework
{
    public static class ContextHelper
    {
        public static AuthenticationProperties GetProperties(HttpContextBase context)
        {
            var authMgr = context.GetOwinContext().Authentication;
            var task = authMgr.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);
            return task.Result.Properties;
        }

        public static Session GetSession(HttpContextBase context)
        {
            var properties = GetProperties(context);

            var sessionJson = (string)properties.Dictionary["Session"];

            return JsonConvert.DeserializeObject<Session>(sessionJson);
        }
    }
}