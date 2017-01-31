using Newtonsoft.Json;
using NtccSteward.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtccSteward.Framework
{
    public class VerifySessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool redirect = false;

            var sessionJson = (string)filterContext.HttpContext.Session["Session"];
            if (string.IsNullOrWhiteSpace(sessionJson))
            {
                redirect = true;
            }
            else
            {
                var session = JsonConvert.DeserializeObject<Session>(sessionJson);
                if (string.IsNullOrWhiteSpace(session?.SessionId))
                {
                    redirect = true;
                }
            }

            if (redirect)
                filterContext.Result = new RedirectResult(string.Format("/Account/Index?targetUrl={0}", filterContext.HttpContext.Request.Url.AbsolutePath));
        }
    }
}