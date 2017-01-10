using NtccSteward.Core.Framework;
using NtccSteward.Core.Interfaces.Messages;
using NtccSteward.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Message
{
    public class MessageVm
    {
        public MessageVm() { }

        public MessageVm(IMessage message)
        {
            this.MessageID = message.MessageID;
            this.MemberID = message.IdentityID;
            this.MemberName = message.IdentityName;
            this.MessageDate = message.MessageDate;
            this.MessageType = (MessageType)Enum.Parse(typeof(MessageType), message.MessageTypeEnumID.ToString());
            this.Subject = message.Subject;
            this.Body = message.Body;
            this.Direction = (MessageDirection)Enum.Parse(typeof(MessageDirection), message.MessageDirectionEnumID.ToString());
        }

        public long MessageID { get; set; }

        // Sender's ID
        public int MemberID { get; set; }

        // Sender's name
        public string MemberName { get; set; }

        /// <summary>
        /// This will serve as the date that it is sent from the user (pastor); and the date it was "sent" from the church member (or date received)
        /// </summary>
        public DateTime MessageDate { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        // Sent/Received
        public MessageDirection Direction { get; set; }

        /// <summary>
        /// SMS, Email, Application Message
        /// </summary>
        public MessageType MessageType { get; set; }
    }
}
