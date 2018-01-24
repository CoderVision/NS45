using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Common.Address
{
    public class EmailProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }

        public int Port { get; set; }
    }
}