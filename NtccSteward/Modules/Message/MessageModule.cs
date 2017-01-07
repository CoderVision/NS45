using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.ViewModels.Message;
using NtccSteward.Core.Interfaces.Messages;
using NtccSteward.Core.Framework.Enums;

namespace NtccSteward.Modules.Message
{
    public class MessageModule : ModuleBase
    {
        public MessageModule()
        {
            this.DisplayText = "Message";

            Initialize();
        }

        private void Initialize()
        {
            GroupList = new List<GroupVm>();
            CorrespondenceList = new List<CorrespondenceVm>();
            MessageList = new List<MessageVm>();

            CreateDevData();
        }

        private void CreateDevData()
        {
            GroupList.Add(new GroupVm() { ID = 1, Name = "Pastors", Description = "All pastors" });
            GroupList.Add(new GroupVm() { ID = 2, Name = "Lay Pastors", Description = "All lay pastors" });
            GroupList.Add(new GroupVm() { ID = 3, Name = "Team Leaders", Description = "All team leaders" });
            GroupList.Add(new GroupVm() { ID = 4, Name = "Soulwinners", Description = "All soulwinners" });

            CorrespondenceList.Add(new CorrespondenceVm() { ID = 1, Name = "Gary Lima" });
            CorrespondenceList.Add(new CorrespondenceVm() { ID = 2, Name = "J.P. Malone" });
            CorrespondenceList.Add(new CorrespondenceVm() { ID = 3, Name = "M.C. Kekel" });
            CorrespondenceList.Add(new CorrespondenceVm() { ID = 4, Name = "Elliot Gesang" });
            CorrespondenceList.Add(new CorrespondenceVm() { ID = 5, Name = "Jim Connor" });


            MessageList.Add(new MessageVm() {
                    MemberID =1, MemberName="Gary Lima"
                    , MessageType= MessageType.TextMessage, Direction= MessageDirection.Received, MessageDate = DateTime.Now
                    , Subject="Msg Subject", Body="Here's a messaage from Gary with new content!"
            });

            MessageList.Add(new MessageVm() {
                MemberID = 2, MemberName = "Phil Kinson"
                    , MessageType = MessageType.TextMessage, Direction = MessageDirection.Sent, MessageDate = DateTime.Now.Subtract(new TimeSpan(0, 10, 0))
                    , Subject="Msg Subject", Body="Here's a messaage from Phil"
            });

            MessageList.Add(new MessageVm() {
                    MemberID =3, MemberName="Gary Lima"
                    , MessageType= MessageType.TextMessage, Direction= MessageDirection.Received, MessageDate = DateTime.Now.Subtract(new TimeSpan(0, 15, 0))
                    , Subject="Msg Subject", Body="Here's a messaage from Gary"
            });

            MessageList.Add(new MessageVm() {
                    MemberID =4, MemberName= "Phil Kinson"
                    , MessageType= MessageType.TextMessage, Direction= MessageDirection.Sent, MessageDate = DateTime.Now.Subtract(new TimeSpan(0, 20, 0))
                    , Subject="Msg Subject", Body= "Here's a messaage from Phil"
            });

            MessageList.Add(new MessageVm() {
                    MemberID =5, MemberName="Gary Lima"
                    , MessageType= MessageType.TextMessage, Direction= MessageDirection.Received, MessageDate = DateTime.Now.Subtract(new TimeSpan(0, 25, 0))
                    , Subject="Msg Subject", Body="Here's a messaage from Gary"
            });
        }


        // list of groups
        public List<GroupVm> GroupList { get; set; }


        // list of recent text messages received (up to n)
        //      option to show more... (n) messages
        public List<CorrespondenceVm> CorrespondenceList { get; set; }

        // list of recent messages received by the selected group or correspondence (up to n)
        //      option to show more... (n) messages
        public List<MessageVm> MessageList { get; set; }


        public ICorrespondence SelectedCorrespondence { get; set; }
    }
}
