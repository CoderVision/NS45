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
        
        [Route("account/GetUsers")]
        public IHttpActionResult GetAccountRequests()
        {
            try
            {
                var acctRequests = this.repository.GetAccountRequests();
                var users = this.repository.GetUsers();
                var churches = this.churchRepository.GetList(false);

                var usersInfo = new
                {
                    config = new { churches = churches },
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
    }
}
