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
using NtccSteward.ViewModels.Church;
using cm = NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Models.Team;
using NtccSteward.ViewModels;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NtccSteward.Controllers
{
    [Authorize]
    public class ChurchController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session;
        private string _uri = "";

        public ChurchController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;

            _uri = "church";
        }

        private void InitSession()
        {
            if (_session == null)
                _session = ContextHelper.GetSession(HttpContext);
        }

        //[VerifySessionAttribute]
        public async Task<ActionResult> Index()
        {
            InitSession();

            var result = await _apiProvider.GetItemAsync(_uri, "");
            var list = _apiProvider.DeserializeJson<List<cm.Church>>(result);

            var metajson = await _apiProvider.GetItemAsync($"{_uri}/metadata");
            var metaList = _apiProvider.DeserializeJson<List<AppEnum>>(metajson);

            var viewModel = new ChurchIndexViewModel() { ChurchList = list, MetaList = metaList };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateChurch(cm.Church church)
        {
            try
            {
                InitSession();

               church.CreatedByUserId = _session.UserId;

                var json = await _apiProvider.PostItemAsync<cm.Church>(_uri, church);
                var ch = _apiProvider.DeserializeJson<cm.Church>(json);

                if (ch != null)
                {
                    return Content("Created: " + ch.id.ToString());
                }
                else
                    return Content("Error creating church");
            }
            catch (Exception ex)
            {
                return Content("Error creating church: " + ex.Message);
            }
        }


        //[VerifySessionAttribute]
        public async Task<ActionResult> Edit(int id)
        {
            InitSession();

            // Get Church Profile
            var mpjson = await _apiProvider.GetItemAsync(_uri, $"id={id}");
            var cp = _apiProvider.DeserializeJson<ChurchProfile>(mpjson);

            // Get Metadata
            var metajson = await _apiProvider.GetItemAsync($"{_uri}/metadata", $"churchId={_session.ChurchId}");
            var metaList = _apiProvider.DeserializeJson<List<AppEnum>>(metajson);

            cp.MetaDataList = metaList.ToList();

            // Get Pastoral Team
            var teamsjson = await _apiProvider.GetItemAsync("team/" + _session.ChurchId);
            var teamList = _apiProvider.DeserializeJson<List<TeamViewModel>>(teamsjson);
            var pastoralTeam = teamList.FirstOrDefault(t => t.TeamTypeEnumId == 17); // pastoral team
            if (pastoralTeam != null)
            {
                var teammatejson = await _apiProvider.GetItemAsync($"team/{pastoralTeam.Id}/teammates");
                var teammateList = _apiProvider.DeserializeJson<List<Teammate>>(teammatejson);
                pastoralTeam.Teammates = teammateList;

                cp.PastoralTeam = pastoralTeam;
            }

            var churchModule = new ChurchModule(cp);

            return View("~/Views/Church/Church.cshtml", churchModule);
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
                        viewModel = new ChurchProfile();
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

        public async Task<ActionResult> DeleteChurch(int id)
        {
            try
            {
                var result = await _apiProvider.DeleteItemAsync($"{_uri}/{id.ToString()}?entityType=55");

                if (string.IsNullOrWhiteSpace(result))
                    return RedirectToAction("Index");
                else
                    return Content("Error deleting church");
            }
            catch (Exception ex)
            {
                return Content("Error deleting church: " + ex.Message);
            }
        }
    }
}
