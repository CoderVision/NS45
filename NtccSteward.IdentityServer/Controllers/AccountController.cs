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
        private readonly IChurchRepository churchRepository;

        public AccountController()
        {
            var cnString = ConfigurationManager.ConnectionStrings["Login"].ConnectionString;
            var pepper = ConfigurationManager.AppSettings["Pepper"];
            this.accountRepository = new AccountRepository(cnString, pepper);

            var cnDefault = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            this.churchRepository = new ChurchRepository(cnDefault);
        }
        // GET: Account
        [HttpGet]
        public ActionResult Index(string signin)
        {
            return View("RequestAccount", new AccountRequest());
        }

        [Route("account/getconfig")]
        [HttpGet]
        public ActionResult GetConfig()
        {
            var churchList = this.churchRepository.GetList(false);

            return Json(new { churchList = churchList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Index(string signin, NtccSteward.Core.Models.Account.AccountRequest acctRequest)  // AccountRequest accountRequest
        {
            if (ModelState.IsValid)
            {
                this.accountRepository.CreateAccountRequest(acctRequest);

                return View("RequestSuccess");
                //return Redirect("~/identity/login?signin=" + signin);  // must return to the signin parameter to maintain the user's session
            }
            else
                return Redirect("~/identity/error");  
        }
    }
}