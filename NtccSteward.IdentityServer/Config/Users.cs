using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace NtccSteward.IdentityServer.Config
{
    public static class Users
    {
        // For SQL Server implementation, see:
        // https://stackoverflow.com/questions/31711014/get-identityserver3-to-use-existing-user-sql-database
        // https://identityserver.github.io/Documentation/docsv2/advanced/userService.html
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>() {
                    new InMemoryUser(){
                        Username="glima",
                        Password="secret",
                        Subject="4633E6C1-05EF-43F3-A24A-C5D51BEB840F",  // specific user's unique identifier
                                            Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Gary"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Lima"),
                        new Claim(Constants.ClaimTypes.Address, "8918 176th Street Ct E, Puyallup, WA  98375")
                                            } } };
        }
    }
};
