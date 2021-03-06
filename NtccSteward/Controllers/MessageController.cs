using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Framework;
using NtccSteward.Core.Models.Account;
using NtccSteward.Modules.Message;
using NtccSteward.Core.Models.Message;
using NtccSteward.Core.Models.Common.Parameters;
using NtccSteward.ViewModels.Message;
using System.Web.Mvc;
using NtccSteward.Core.Framework;

namespace NtccSteward.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IApiProvider _apiProvider;
        private Session _session = null;

        public MessagesController(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        private void InitSession()
        {
            if (_session == null)
                _session = ContextHelper.GetSession(HttpContext);
        }

        private string SubmitRequest<T>(string path, T model)
        {
            var task = _apiProvider.PostItemAsync<T>(path, model);
            task.Wait();

            return task.Result;
        }

        public ActionResult Index()
        {
            var messageModule = LoadMessageModule();

            return View(messageModule);
        }


        private MessageModule LoadMessageModule()
        {
            var messageModule = new MessageModule();

            int churchId = _session?.ChurchId ?? 3; // default to Graham
            var param = new GetCorrespondenceParameter(churchId, (int)MessageType.TextMessage, 100);

            // The Web Api GET method needs to be changed to remove the body from the message, and use query string instead
            //var correspondence = SubmitRequest<GetCorrespondenceParameter>("/api/message/GetCorrespondence", param);
            //var correspondence = await _apiProvider.GetItemAsync<GetCorrespondenceParameter>(Request, "/api/message/GetCorrespondence", mp);
            var correspondence = "";

            var list = _apiProvider.DeserializeJson<List<Correspondence>>(correspondence);
            messageModule.CorrespondenceList = list.Select<Correspondence, CorrespondenceVm>(c => new CorrespondenceVm(c)).ToList();

            var groups = SubmitRequest<int>("/api/message/GetGroups", churchId);
            var grps = _apiProvider.DeserializeJson<List<RecipientGroup>>(groups);
            messageModule.GroupList = grps.Select<RecipientGroup, GroupVm>(c => new GroupVm(c)).ToList();

            //messageModule.MessageList
            int memberID = list.FirstOrDefault()?.ID ?? 0; // most received correspondence
            messageModule.MessageList = LoadMessageList(memberID);

            return messageModule;
        }

        public ActionResult MessageList(int id)
        {
            var messageList = LoadMessageList(id);

            return PartialView("~/Views/Messages/_MessageList.cshtml", messageList);
        }

        private List<MessageVm> LoadMessageList(int id)
        {
            var gmp = new GetMessagesParameter(id, (int)MessageType.TextMessage, 100);
            var messages = SubmitRequest<GetMessagesParameter>("/api/message/GetMessages", gmp);
            var msgs = _apiProvider.DeserializeJson<List<Message>>(messages);
            var list = msgs.Select<Message, MessageVm>(c => new MessageVm(c)).ToList();
            return list;
        }


        public ActionResult SendSmsMsg(int id, string msg)
        {
            var msgVm = new MessageVm();
            msgVm.Body = msg;
            msgVm.Direction = MessageDirection.Sent;
            msgVm.MemberID = id;
            msgVm.MemberName = "Member Name";
            msgVm.MessageDate = DateTime.Now;  

            // Left off here.  We need to save the msg to the databse, and select the necessary information to populate the return view model.

            return PartialView("~/Views/Messages/_FormattedMsg.cshtml", msgVm);
        }
    }
}