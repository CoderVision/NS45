using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class MailConfiguration
    {
        public string Name { get; set; }

        public string Server { get; set; }

        public int Port { get; set; }

        public bool Active { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int EmailConfigurationProfileId { get; set; }
    }
}