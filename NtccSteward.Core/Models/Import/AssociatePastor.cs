using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class AssociatePastor
    {
        public int AssocId { get; set; }
        public string Initials { get; set; }
        public string Name { get; set; }
        public bool Current { get; set; }

        public int IdentityId { get; set; }
    }
}