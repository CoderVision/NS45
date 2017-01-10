using NtccSteward.Core.Models.Account;
using NtccSteward.Framework;
using NtccSteward.ViewModels.Account;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    public class AccountController : Controller
    {
        private IApiProvider _apiProvider;

        public AccountController(IApiProvider apiProvider) //, UserProvider securityManager, SignInManager<Login> loginManager
        {
            _apiProvider = apiProvider;
        }

        public ActionResult Index()
        {
            var loginVm = new LoginVm();

            var loginEmail = Request.Cookies["email"];
            if (loginEmail != null)
            {
                loginVm.Email = loginEmail.Value;
                loginVm.Remember = true;
            }

            var churchId = Request.Cookies["churchId"];
            if (churchId != null)
                loginVm.ChurchId = Convert.ToInt32(churchId.Value);

            return View("/Views/Account/Login.cshtml", loginVm);
        }


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVm login)
        {
            if (ModelState.IsValid)
            {
                if (login.Remember)
                    Response.Cookies.Add(new HttpCookie("email", login.Email));

                // Church:  default to Graham, but in the future:
                //     After user enters email, then async get a list of churches they have access (roles) to and have them choose one,
                // also, cache the selected church and restore default, if there are multiple

                var session = await _apiProvider.PostItemAsync<Login>(Request, "api/account/Login", new Login(login));

                if (string.IsNullOrWhiteSpace(session))
                {
                    
                    return Json(new PostResponse(Url.Action("Index", "Account"), "Login attempt failed, please try again.", false));
                }
                else
                {
                    HttpContext.Session["Session"] = session;

                    // return Json(new PostResponse(Url.Action("Index", "Member"), string.Empty, true));
                    return RedirectToAction("Index", "Member");
                }
            }
            else
                return Json(
                    new PostResponse(Url.Action("Index", "Account"), "Credentials were not valid, please try again.", false)
                );
        }


        public ActionResult RequestAccount()
        {
            return View("/Views/Account/RequestAccount.cshtml");
        }


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitAccountRequest(RequestAccountVm login)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiProvider.PostItemAsync<AccountRequest>(Request, "/Api/Account/CreateAccountRequest", new AccountRequest(login));

                if (Convert.ToInt32(result) > 0)
                    return View("/Views/Account/AccountRequestComplete.cshtml");
                else
                    return new ContentResult() { Content = "Accout request did not complete, please try again." };
            }
            else
                return new ContentResult();
        }


        public ActionResult Terms()
        {
            return View("/Views/Account/Terms.cshtml");
        }
    }
}
