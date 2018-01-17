using IdentityServer3.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Models;
using System.Threading.Tasks;

namespace NtccSteward.IdentityServer.Services
{
    public class UserService : UserServiceBase
    {
        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var userName = context.UserName;
            var password = context.Password;

            // *** Here is where you hash the password before sending it to the database.

            // use the repository to get the user.


            /// Finish the Custom User Store with Kevin:
            /// // https://app.pluralsight.com/player?course=oauth2-openid-connect-angular-aspdotnet&author=kevin-dockx&name=oauth2-openid-connect-angular-aspdotnet-m08&clip=1&mode=live

            return base.AuthenticateLocalAsync(context);
        }
    }
}