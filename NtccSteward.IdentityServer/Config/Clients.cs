using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NtccSteward.IdentityServer.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[] {
                new  Client
                {
                    ClientId="NtccStewardImplicit",
                    ClientName="Ntcc Steward Log-in Service",
                    Flow = Flows.Implicit,
                    AllowAccessToAllScopes = true,
                    RedirectUris = new List<string>{
                        ConfigurationManager.AppSettings["NtccStewardAngularApp"],
                        ConfigurationManager.AppSettings["NtccStewardAngularApp"] + "silentRefresh.html"
                    },
                    IdentityTokenLifetime = 300, // 5 minutes,
                    AccessTokenLifetime = 3600, // 1 hour
                    PostLogoutRedirectUris = new List<string>{
                        ConfigurationManager.AppSettings["NtccStewardAngularApp"]  // have to think about this carefully, because redirecting to the app may cause it to redirect to the token service
                    }
                }
            };
        }
    }
}