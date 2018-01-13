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
                    ClientName="Ntcc Steward Security Token Service",
                    Flow = Flows.Implicit,
                    AllowAccessToAllScopes = true,
                    RedirectUris = new List<string>{
                        ConfigurationManager.AppSettings["NtccStewardAngularApp"] 
                    }
                }
            };
        }
    }
}