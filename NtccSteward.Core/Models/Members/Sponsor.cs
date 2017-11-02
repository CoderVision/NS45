using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Members
{
    public class Sponsor
    {
        public int MemberId { get; set; }
        public int SponsorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}