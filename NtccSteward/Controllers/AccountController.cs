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
using NtccSteward.Core.Models.Church;
using NtccSteward.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    public class AccountController : Controller
    {
        private IApiProvider _apiProvider;
        private AppUserManager _appUserManager;
        private AppRoleManager _appRoleManager;

        public AccountController(IApiProvider apiProvider, AppUserManager appUserManager, AppRoleManager appRoleManager)
        {
            _apiProvider = apiProvider;
            _appUserManager = appUserManager;
            _appRoleManager = appRoleManager;
        }

        public async Task<ActionResult> Index()
        {
            try
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
            }catch (Exception ex)
            {
                return Content("The following error occurred while trying to load the login page:  " + ex.Message);
            }
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
                }

                var sessionJson = await _apiProvider.PostItemAsync<Login>("account/Login", new Login(login));

                if (string.IsNullOrWhiteSpace(sessionJson)
                    || sessionJson.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    TempData["loginError"] = "Login attempt failed, please try again.";
                }
                else
                {
                    HttpContext.Session["Session"] = sessionJson;

                    CreateIdentity(sessionJson);

                    //return RedirectToAction("Index", "Member", new { statusIds = "49-50", page = 1, pageSize = 1000, showAll = false });
                    return RedirectToAction("Index", "Church");
                }
            }
            else
            {
                TempData["loginError"] = "Please enter a valid username and password.";
            }

            return RedirectToAction("Index");
        }

        private void CreateIdentity(string sessionJson)
        {
            var session = _apiProvider.DeserializeJson<Session>(sessionJson);
            var appUser = new AppUser(session.UserId.ToString(), "User");
            var task = _appUserManager.CreateIdentityAsync(appUser, DefaultAuthenticationTypes.ApplicationCookie);
            task.Wait();
            var claim = task.Result;

            var authMgr = HttpContext.GetOwinContext().Authentication;
            var authProperties = new AuthenticationProperties() { IsPersistent = false };
            authProperties.Dictionary["Session"] = sessionJson;

            foreach (var role in session.Roles)
            {
                var appRole = _appRoleManager.FindById(role.RoleID.ToString());
                if (appRole != null)
                {
                   _appUserManager.AddToRoleAsync(appUser.Id.ToString(), appRole.Name);
                }
            }

            authMgr.SignIn(authProperties, claim);
        }


        public async Task<ActionResult> RequestAccount()
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

            var churchJson = await _apiProvider.GetItemAsync("church", "page=1&pageSize=10000&showAll=false");

            login.ChurchList = _apiProvider.DeserializeJson<List<Church>>(churchJson);

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
