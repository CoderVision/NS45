using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Church
{
    public class ChurchProfileMetadata
    {
        public ChurchProfileMetadata()
        {
            Enums = new List<AppEnum>();
            EmailProviders = new List<EmailProvider>();
        }
        public List<AppEnum> Enums { get; set; }

        public List<EmailProvider> EmailProviders { get; set; }
    }
}