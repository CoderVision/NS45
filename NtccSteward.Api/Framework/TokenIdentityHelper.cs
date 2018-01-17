using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace NtccSteward.Api.Framework
{
    public static class TokenIdentityHelper
    {
        // example of how to get the user's id
        //var userId = TokenIdentityHelper.GetOwnerIdFromToken();
        public static string GetOwnerIdFromToken()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;

            if (identity == null)
                return null;

            var issuerFromIdentity = identity.FindFirst("iss");
            var subFromIdentity = identity.FindFirst("sub");
            if (issuerFromIdentity == null || subFromIdentity == null)
                return null;

            return issuerFromIdentity.Value + subFromIdentity.Value;
        }
    }
}