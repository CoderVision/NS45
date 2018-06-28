using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class LayPastor
    {
        public int LayPastorID { get; set; }
        public string LayPastorName { get; set; }
        public string Initials { get; set; }
        public bool Current { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public int IdentityId { get; set; }
    }
}