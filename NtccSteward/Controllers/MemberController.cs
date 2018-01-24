using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Modules.Members;
using NtccSteward.Modules;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.ViewModels.Common.Address;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;
using cm = NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Common.Parameters;
using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Factories;
using System.Web;
using System.Web.Mvc;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.ViewModels.Member;
using NtccSteward.Core.Models.Church;
using NtccSteward.Models;

namespace NtccSteward.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session = null;
        private string _uri = "";

        public MemberController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;

            _uri = "members";

        }

        private void InitSession()
        {
            if (_session == null)
                _session = ContextHelper.GetSession(HttpContext);
        }

        //[VerifySessionAttribute]
        public async Task<ActionResult> Index(string statusIds, int page = 1, int pageSize = 1000)
        {
            try
            {
                InitSession();

                var queryString = $"churchId={_session.ChurchId}&statusIds={statusIds}&page={page}&pageSize={pageSize}";
                var result = await _apiProvider.GetItemAsync(_uri, queryString);
                var list = _apiProvider.DeserializeJson<List<cm.Member>>(result);

                var metajson = await _apiProvider.GetItemAsync($"{_uri}/metadata", $"churchId={_session.ChurchId}");
                var metaList = _apiProvider.DeserializeJson<List<AppEnum>>(metajson);

                var churchjson = await _apiProvider.GetItemAsync("church/" + _session.ChurchId);
                var church = _apiProvider.DeserializeJson<Church>(churchjson);

                var viewModel = new MemberIndexViewModel() { MemberList = list, MetaList = metaList };
                viewModel.Title = $"{church.Name} Member List";

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Content("Error loading Member list: " + ex.Message);
            }
        }

        // gets a member
        //[VerifySessionAttribute]
        public async Task<ActionResult> Edit(int id)
        {
            InitSession();

            var mpjson = await _apiProvider.GetItemAsync(_uri, $"id={id}");
            var mp = _apiProvider.DeserializeJson<cm.MemberProfile>(mpjson);

            var memberProfileModule = this.CopyMemberProfileToVm(mp);

            var metajson = await _apiProvider.GetItemAsync($"{_uri}/metadata", $"churchId={_session.ChurchId}");
            var metaList = _apiProvider.DeserializeJson<List<AppEnum>>(metajson);

            memberProfileModule.MetaDataList = metaList.ToList();

            var shell = new MemberModule(memberProfileModule);

            return View("~/Views/Member/Member.cshtml", shell);
        }


        public ActionResult CreateAddress(string addyType, int memberId)
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

            return PartialView("/Views/Shared/AddressInfo/" + partialView, info);
        }


        public async Task<ActionResult> GetView(string viewName, int memberId)
        {
            ModuleBase viewModel = null;
            var viewPath = "/Views/Member/";

            switch (viewName.ToLower())
            {
                case "awards":
                    {
                        viewModel = new MemberAwards(memberId);
                        viewPath += "_MemberAwards.cshtml";
                        break;
                    }
                case "activity":
                    {
                        viewModel = new MemberContacts(memberId);
                        viewPath += "_MemberActivity.cshtml";
                        break;
                    }
                case "messages":
                    {
                        viewModel = new MemberMessages(memberId);
                        viewPath += "_MemberMessages.cshtml";
                        break;
                    }
                case "notes":
                    {
                        viewModel = new MemberNotes(memberId);
                        viewPath += "_MemberNotes.cshtml";
                        break;
                    }
                case "history":
                    {
                        viewModel = new MemberActivity(memberId);
                        viewPath += "_MemberHistory.cshtml";
                        break;
                    }

                
                default:
                    {
                        var mpjson = await _apiProvider.GetItemAsync("/api/member/GetProfile", $"id={memberId}");
                        var mp = _apiProvider.DeserializeJson<cm.MemberProfile>(mpjson);
                        viewModel = CopyMemberProfileToVm(mp);
                        viewPath += "_MemberProfile.cshtml";
                        break;
                    }
            }

            return PartialView(viewPath, viewModel);
        }

        // Why are we copying?  
        [NonAction]
        private MemberProfile CopyMemberProfileToVm(cm.MemberProfile mp)
        {
            var mCopyDepot = new MemberFactory();
            var addyCopyDepot = new AddressInfoFactory();

            var mpVm = mCopyDepot.CreateMemberProfile<MemberProfile>(mp);

            foreach (var addy in mp.AddressList)
                mpVm.AddressList.Add(addyCopyDepot.CreateAddress<AddressVm>(addy));

            foreach (var addy in mp.PhoneList)
                mpVm.PhoneList.Add(addyCopyDepot.CreatePhone<PhoneVm>(addy));

            foreach (var addy in mp.EmailList)
                mpVm.EmailList.Add(addyCopyDepot.CreateEmail<EmailVm>(addy));

            foreach (var attr in mp.CustomAttributeList)
                mpVm.CustomAttributeList.Add(attr);

            //foreach (var meta in mp.MetaDataList)
            //    mpVm.MetaDataList.Add(meta);

            return mpVm;
        }

        //[ValidateAntiForgeryToken]
        //[VerifySessionAttribute]
        public async Task<ActionResult> SaveProfile(MemberProfile memberProfile)
        {
            InitSession();

            var mFactory = new MemberFactory();
            var addyFactory = new AddressInfoFactory();

            var mp = mFactory.CreateMemberProfile<cm.MemberProfile>(memberProfile);

            foreach (var addy in memberProfile.AddressList)
            {
                var a = addyFactory.CreateAddress<Address>(addy);
                a.ModifiedByIdentityId = _session.UserId;
                mp.AddressList.Add(a);
            }

            foreach (var addy in memberProfile.PhoneList)
            {
                var p = addyFactory.CreatePhone<Phone>(addy);
                p.ModifiedByIdentityId = _session.UserId;
                mp.PhoneList.Add(p);
            }
                
            foreach (var addy in memberProfile.EmailList)
            {
                var e = addyFactory.CreateEmail<Email>(addy);
                e.ModifiedByIdentityId = _session.UserId;
                mp.EmailList.Add(e);
            }

            var json = await _apiProvider.PutItemAsync<cm.MemberProfile>($"{_uri}/{ memberProfile.MemberId}", mp);
            
            try
            {
                var m = _apiProvider.DeserializeJson<cm.MemberProfile>(json);
                if (m != null)
                {
                    // reload page, so that any new id's can be loaded.
                    return RedirectToAction("Edit", new { id = m.MemberId });
                }
                return Content("Error saving profile");
            }
            catch (Exception ex)
            {
                return Content("Error saving profile: " + ex.Message);
            }
        }

        //[HttpDelete]
        public async Task<ActionResult> RemoveAddress(int addressId)
        {
            var result = await _apiProvider.DeleteItemAsync($"{_uri}/{addressId}", "entityType=61");

            if (string.IsNullOrWhiteSpace(result))
                return Content("");
            else
                return Content("Error removing address");
        }

        //[VerifySessionAttribute]
        [HttpPost]
        public async Task<ActionResult> CreateMember(cm.NewMember member)
        {
            InitSession();

            member.ChurchId = _session?.ChurchId ?? 3; // default to graham
            member.CreatedByUserId = _session?.UserId ?? 0; // default to system

            var mjson = await _apiProvider.PostItemAsync<cm.NewMember>(_uri, member);
            var m = _apiProvider.DeserializeJson<cm.NewMember>(mjson);

            return Content("Created " + m.id.ToString());
        }

        //[HttpPost]
        public async Task<ActionResult> DeleteMember(int id)
        {
            try
            {
                var result = await _apiProvider.DeleteItemAsync($"{_uri}/{id.ToString()}?entityType=56");

                if (string.IsNullOrWhiteSpace(result))
                    return RedirectToAction("Index", new { statusIds = "49-50" });
                else
                    return Content("Error deleting member");
            }
            catch(Exception ex)
            {
                return Content("Error deleting member: " + ex.Message);
            }
        }
    }
}
