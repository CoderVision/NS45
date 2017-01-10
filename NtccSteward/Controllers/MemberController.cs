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

namespace NtccSteward.Controllers
{
    public class MemberController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session = null;

        public MemberController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }           

        private Session InitSession()
        {
            if (_session == null)
            {
                var sessionJson = (string)HttpContext.Session["Session"];
                _session = _apiProvider.DeserializeJson<Session>(sessionJson);
            }

            return _session;
        }

        public ActionResult Index()
        {
            InitSession();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
                return View();
        }

        // gets a member
        public async Task<ActionResult> Member(int id)
        {
            InitSession();

            if (string.IsNullOrWhiteSpace(_session?.SessionId))
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                var mpjson = await _apiProvider.GetItemAsync(Request, "/api/member/GetProfile", $"id={id}");

                var mp = _apiProvider.DeserializeJson<cm.MemberProfile>(mpjson);
                var memberProfileModule = this.CopyMemberProfileToVm(mp);

                var shell = new MemberModule(memberProfileModule);

                return View(shell);
            }
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
                        var mpjson = await _apiProvider.GetItemAsync(Request, "/api/member/GetProfile", $"id={memberId}");
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

            foreach (var meta in mp.MetaDataList)
                mpVm.MetaDataList.Add(meta);

            return mpVm;
        }

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveProfile(MemberProfile memberProfile)
        {
            InitSession();

            var mCopyDepot = new MemberFactory();
            var addyCopyDepot = new AddressInfoFactory();

            var mp = mCopyDepot.CreateMemberProfile<cm.MemberProfile>(memberProfile);

            foreach (var addy in memberProfile.AddressList)
            {
                var a = addyCopyDepot.CreateAddress<Address>(addy);
                a.ModifiedByIdentityId = _session.UserId;
                mp.AddressList.Add(a);
            }

            foreach (var addy in memberProfile.PhoneList)
            {
                var p = addyCopyDepot.CreatePhone<Phone>(addy);
                p.ModifiedByIdentityId = _session.UserId;
                mp.PhoneList.Add(p);
            }
                
            foreach (var addy in memberProfile.EmailList)
            {
                var e = addyCopyDepot.CreateEmail<Email>(addy);
                e.ModifiedByIdentityId = _session.UserId;
                mp.EmailList.Add(e);
            }

            var memberId = await _apiProvider.PutItemAsync<cm.MemberProfile>(Request, "/api/member/SaveMemberProfile", mp);

            return  Json(memberId);
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveAddress(int addressId)
        {
            bool isNew = addressId < 0;

            // delete from database
            //var memberId = SubmitRequest<cm.MemberProfile>("/api/member/SaveMemberProfile", mp);

            //return new NoContentResult(); // this prevents navigation to another page.
            return Json(isNew);
            //return Content();
        }


        [HttpPost]
        //[HttpPost("CreateMember")]
        public async Task<ActionResult> CreateMember(cm.NewMember member)
        {
            InitSession();

            member.ChurchId = _session?.ChurchId ?? 3; // default to graham
            member.CreatedByUserId = _session?.UserId ?? 0; // default to system

            var memberId = await _apiProvider.PutItemAsync<cm.NewMember>(Request, "/api/member/CreateMember", member);

            string url = Url.Action("CreateMember", "Members", memberId);

            var result = new
            {
                url = url,
                memberId = memberId
            };

            return Json(result);
        }
    }
}
