using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.IdentityServer.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                 StandardScopes.ProfileAlwaysInclude,
                 StandardScopes.Address,
                 StandardScopes.OfflineAccess,
                new Scope
                {
                    Name="ApplicationAccess",
                    DisplayName = "Application Usage",
                    Description = "Allow users to use the application",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role", false)  // false = do not include in identity token
                    }
                },
                new Scope
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "Allow the application to see your roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>{
                        new ScopeClaim("role", true)
                    }
                }
            };
        }
    }
}

