using NtccSteward.Core.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class Message : IMessage
    {
        public long MessageID { get; set; }

        public int IdentityID { get; set; }

        public string IdentityName { get; set; }

        public DateTime MessageDate { get; set; }

        /// <summary>
        /// Text, Email
        /// </summary>
        public int MessageTypeEnumID { get; set; }

        /// <summary>
        /// Sent, Received
        /// </summary>
        public int MessageDirectionEnumID { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
