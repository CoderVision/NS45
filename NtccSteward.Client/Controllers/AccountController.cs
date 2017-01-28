using NtccSteward.Core.Models.Account;
using NtccSteward.Framework;
using NtccSteward.ViewModels.Account;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using NtccSteward.ViewModels.Church;
using System.Collections.Generic;
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

        public async Task<ActionResult> Index()
        {
            var loginVm = new LoginVm();

            var json = await _apiProvider.GetItemAsync("church", "page=1&pageSize=10000&showAll=false");

            loginVm.ChurchList = _apiProvider.DeserializeJson<List<ChurchVm>>(json);

            var loginEmail = Request.Cookies["email"];
            if (loginEmail != null)
            {
                loginVm.Email = loginEmail.Value;
                loginVm.Remember = true;
            }

            var churchId = Request.Cookies["churchId"];
            if (churchId != null)
                loginVm.ChurchId = Convert.ToInt32(churchId.Value);

            var password = Request.Cookies["password"];
            if (password != null)
                loginVm.Password = password.Value;

            if (TempData["loginError"] != null)
                ModelState.AddModelError("loginError", TempData["loginError"].ToString());

            return View("/Views/Account/Login.cshtml", loginVm);
        }


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVm login)
        {
            if (ModelState.IsValid)
            {
                if (login.Remember)
                {
                    Response.Cookies.Add(new HttpCookie("email", login.Email));
                    Response.Cookies.Add(new HttpCookie("churchId", login.ChurchId.ToString()));
                    Response.Cookies.Add(new HttpCookie("password", login.Password));
                }

                var session = await _apiProvider.PostItemAsync<Login>("account/Login", new Login(login));

                if (string.IsNullOrWhiteSpace(session) 
                    || session.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    TempData["loginError"] = "Login attempt failed, please try again.";
                }
                else
                {
                    HttpContext.Session["Session"] = session;

                    return RedirectToAction("Index", "Member", new { churchId = login.ChurchId, statusId = 49, page = 1, pageSize = 1000, showAll = false });
                }
            }
            else
            {
                TempData["loginError"] = "Please enter a valid username and password.";
            }

            return RedirectToAction("Index");
        }


        public ActionResult RequestAccount()
        {
            RequestAccountVm login;
            if (TempData["login"] != null)
            {
                login = (RequestAccountVm)TempData["login"];

                if (TempData["sarError"] != null)
                    ModelState.AddModelError("sarError", TempData["sarError"].ToString());
            }
            else
                login = new RequestAccountVm();

            return View("/Views/Account/RequestAccount.cshtml", login);
        }


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitAccountRequest(RequestAccountVm login)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiProvider.PostItemAsync<AccountRequest>("Account/CreateAccountRequest", new AccountRequest(login));

                if (!string.IsNullOrWhiteSpace(result))
                {
                    return View("/Views/Account/AccountRequestComplete.cshtml");
                }
                else
                {
                    TempData["sarError"] = "Accout request did not complete, please try again.";
                }
            }
            else
            {
                TempData["sarError"] = "Verify all fields are correct and try again.";
            }

            return RedirectToAction("RequestAccount");
        }


        public ActionResult Terms()
        {
            return View("/Views/Account/Terms.cshtml");
        }
    }
}
