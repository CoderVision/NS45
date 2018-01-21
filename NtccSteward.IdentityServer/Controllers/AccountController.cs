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
        [HttpGet]
        public ActionResult Index(string signin)
        {
            return View("RequestAccount", new AccountRequest());
        }

        [HttpPost]
        public ActionResult Index(string signin, Login login)  // AccountRequest accountRequest
        {
            if (ModelState.IsValid)
            {
                //return View("RequestAccount", new AccountRequest());
                return Redirect("~/identity/login?signin=" + signin);  // must return to the signin parameter to maintain the user's session
            }
            return View();
        }
    }
}