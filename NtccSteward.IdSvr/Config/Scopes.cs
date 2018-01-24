using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.IdSvr.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
            {
                new Scope
                {
                    Name="Application",
                    DisplayName = "Application Usage",
                    Description = "Allow users to use the main application",
                    Type = ScopeType.Resource
                }
            };
        }
    }
}