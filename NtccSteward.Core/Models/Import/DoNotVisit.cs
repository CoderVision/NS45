using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class DoNotVisit
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Development { get; set; }
        public string Date { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }

        public int IdentityId { get; set; }
    }
}