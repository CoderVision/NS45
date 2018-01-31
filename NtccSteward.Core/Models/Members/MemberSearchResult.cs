using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Members
{
    public class MemberSearchResult
    {
        public int MemberId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}".Trim(); }
        }
        public int ChurchId { get; set; }

        public string ChurchName { get; set; }
    }
}