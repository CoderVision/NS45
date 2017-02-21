using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NtccSteward.Models
{
    public class AppUser : IUser
    {
        public AppUser(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public string Id { get; private set; }

        public string UserName { get; set; }

        internal Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUserManager userManager)
        {
            throw new NotImplementedException();
        }
    }
}