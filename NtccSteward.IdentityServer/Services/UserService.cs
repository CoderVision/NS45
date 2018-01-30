using IdentityServer3.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Models;
using System.Threading.Tasks;
using NtccSteward.Repository;
using static IdentityServer3.Core.Constants;
using IdentityServer3.Core.Extensions;
using System.Security.Claims;
using IdentityServer3.Core;
using NtccSteward.Core.Models.Account;

namespace NtccSteward.IdentityServer.Services
{
    public class UserService : UserServiceBase
    {
        private readonly IAccountRepository accountRepository;

        public UserService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            // *** Here is where you hash the password before sending it to the database.

            // use the repository to get the user.
            var user = this.accountRepository.Login(context.UserName, context.Password);

            if (user == null)
            {
                context.AuthenticateResult = new AuthenticateResult("Invalid user name or password");
                return Task.FromResult(0);
            }

            context.AuthenticateResult = new AuthenticateResult(user.Subject,
                user.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue);

            return base.AuthenticateLocalAsync(context);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject == null)
                throw new ArgumentException("Subject");

            var subjectId = context.Subject.GetSubjectId();

            int userId = 0;
            if (!int.TryParse(subjectId, out userId))
                throw new ArgumentException("Subject");

            var user = this.accountRepository.GetUser(userId);

            var claims = new List<Claim> {
                new Claim(Constants.ClaimTypes.Subject, user.Subject)
            };

            claims.AddRange(user.UserClaims.Select<UserClaim, Claim>(uc => new Claim(uc.ClaimType, uc.ClaimValue)));

            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
            }

            context.IssuedClaims = claims;

            return Task.FromResult(0);
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null)
                throw new ArgumentException("Subject");

            var subjectId = context.Subject.GetSubjectId();

            int userId = 0;
            if (!int.TryParse(subjectId, out userId))
                throw new ArgumentException("Subject");

            var user = this.accountRepository.GetUser(userId);

            context.IsActive = (user != null) && user.IsActive;

            return Task.FromResult(0);
        }
    }
}