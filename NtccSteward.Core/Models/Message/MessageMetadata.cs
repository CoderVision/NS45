using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Message
{
    public class MessageMetadata
    {
        public MessageMetadata()
        {
            Enums = new List<AppEnum>();
            Churches = new List<Church.Church>();
        }

        public List<AppEnum> Enums { get; set; }
        public List<Church.Church> Churches { get; set; }
    }
}