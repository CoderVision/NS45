using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Account
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<int> ChurchIds { get; set; }
    }
}