using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Message
{
    public class SmsConfiguration
    {
        public string Token { get; set; }
        public string Sid { get; set; }
        public string PhoneNumber { get; set; }
    }
}