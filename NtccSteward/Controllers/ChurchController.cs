using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.ViewModels.Common.Address;
using NtccSteward.Modules;
using NtccSteward.Modules.Church;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;
using System.Web.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    public class ChurchController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session;

        public ChurchController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        private void Initialize()
        {
            if (_session == null)
            {
                var sessionJson = (string)HttpContext.Session["Session"];
                _session = _apiProvider.DeserializeJson<Session>(sessionJson);
            }
        }

        public ActionResult Index()
        {
            Initialize();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
                return View();
        }

        public ActionResult Church(int id)
        {
            Initialize();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                var shell = new ChurchModule(id);

                return View(shell);
            }
        }

        public ActionResult CreateAddress(string addyType, int churchId)
        {
            AddressInfoVm info = null;
            var partialView = string.Empty;

            switch (addyType.ToLower())
            {
                case "address":
                    info = new AddressVm();
                    partialView = "_Address.cshtml";
                    break;
                case "email":
                    info = new EmailVm();
                    partialView = "_Email.cshtml";
                    break;
                case "phone":
                    info = new PhoneVm();
                    partialView = "_Phone.cshtml";
                    break;
            }

            info.Id = new Random().Next(-100, -1);
            info.IdentityId = churchId;

            return PartialView("/Views/Shared/AddressInfo/" + partialView, info);
        }


        public ActionResult GetView(string viewName, int churchId)
        {
            ModuleBase viewModel = null;
            var viewPath = "/Views/Church/";

            switch (viewName.ToLower())
            {
                case "attributes":
                    {
                        viewModel = new ChurchAttributes(churchId);
                        viewPath += "_ChurchAttributes.cshtml";
                        break;
                    }
                case "communication":
                    {
                        viewModel = new ChurchCommunication(churchId);
                        viewPath += "_ChurchCommunication.cshtml";
                        break;
                    }
                case "notes":
                    {
                        viewModel = new ChurchNotes(churchId);
                        viewPath += "_ChurchNotes.cshtml";
                        break;
                    }
                case "history":
                    {
                        viewModel = new ChurchHistory(churchId);
                        viewPath += "_ChurchHistory.cshtml";
                        break;
                    }
                case "teams":
                    {
                        viewModel = new ChurchTeams(churchId);
                        viewPath += "_ChurchTeams.cshtml";
                        break;
                    }
                default:
                    {
                        viewModel = new ChurchProfile(churchId);
                        viewPath += "_ChurchProfile.cshtml";
                        break;
                    }
            }

            //viewModel.Load();

            return PartialView(viewPath, viewModel);
        }


        //[ValidateAntiForgeryToken]
        public ActionResult SaveProfile(ChurchProfile churchProfile)
        {
            //if (ModelState.IsValid)
            //{
            ViewBag.Msg = "Saved";
            //}
            return new ContentResult() { Content="Saved" }; // this prevents navigation to another page.
        }

        public ActionResult RemoveAddress(int addressId)
        {
            bool isNew = addressId < 0;

            // delete from database

            return new ContentResult(); // this prevents navigation to another page.
        }
    }
}
