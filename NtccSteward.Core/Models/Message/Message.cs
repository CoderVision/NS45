﻿using NtccSteward.Core.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class Message : IMessage
    {
        public long Id { get; set; }

        public int RecipientGroupId { get; set; }

        public DateTimeOffset MessageDate { get; set; }

        /// <summary>
        /// Sent, Received
        /// </summary>
        public int MessageDirectionEnumID { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public int ChurchId { get; set; }
    }
}
