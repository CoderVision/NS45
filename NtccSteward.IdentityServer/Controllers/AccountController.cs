using NtccSteward.Core.Models.Account;
using NtccSteward.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtccSteward.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository accountRepository;

        public AccountController()
        {
            var cnString = ConfigurationManager.ConnectionStrings["Login"].ConnectionString;
            var pepper = ConfigurationManager.AppSettings["Pepper"];
            this.accountRepository = new AccountRepository(cnString, pepper);
        }
        // GET: Account
        [Route("login")]
        [HttpGet]
        public ActionResult Index(string signin)
        {
            return View("RequestAccount", new AccountRequest());
        }

        [Route("login")]
        [HttpGet]
        public ActionResult Index2(string signin, Login login)  // AccountRequest accountRequest
        {
            if (ModelState.IsValid)
            {
                return View("RequestAccount", new AccountRequest());
            }
            return View("RequestAccount", new AccountRequest());
        }
    }
}