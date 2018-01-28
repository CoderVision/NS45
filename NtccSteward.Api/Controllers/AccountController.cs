using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Models;
using NtccSteward.Repository;
using NtccSteward.Core.Framework;
using System.Text;
using NtccSteward.Api.Framework;
using System.Net;
using NtccSteward.Core.Models.Account;
using System.Net.Http;
using System.Web.Http;
using System.Web;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Api.Controllers
{
    //[RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private readonly IAccountRepository repository = null;
        private readonly ILogger logger;
        private readonly IChurchRepository churchRepository;

        public AccountController(IAccountRepository repository, IChurchRepository churchRepository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
            this.churchRepository = churchRepository;
        }


        [Route("account/GetUsers/{active}")]
        [HttpGet]
        public IHttpActionResult GetAccountRequests(bool active)
        {
            try
            {
                var acctRequests = this.repository.GetAccountRequests();
                var roles = this.repository.GetRoles();
                var users = this.repository.GetUsers(active);
                var churches = this.churchRepository.GetList(false);

                var usersInfo = new
                {
                    config = new { churches = churches, roles = roles },
                    acctRequests = acctRequests,
                    users = users,
                };

                return Ok(usersInfo);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(GetAccountRequests));

                return InternalServerError(ex);
            }
        }

        [Route("account/processAccountRequest")]
        [HttpPost]
        public IHttpActionResult ProcessAccountRequest(AccountRequest accountRequest)
        {
            try
            {
                // example of how to get the user's id
                var userId = TokenIdentityHelper.GetOwnerIdFromToken();

                accountRequest.ReviewerUserId = userId;

                var result = this.repository.ProcessAccountRequest(accountRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorHelper.ProcessError(this.logger, ex, nameof(GetAccountRequests));

                return InternalServerError(ex);
            }
        }
    }
}
