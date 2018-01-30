using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Account
{
    public class UserProfile
    {
        public UserProfile()
        {
            ChurchIds = new List<int>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private string fullName;

        public string FullName
        {
            get { return $"{FirstName} {LastName}".Trim(); }
        }

        public string Line1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string Zip { get; set; }
        public string Email { get; set; }

        public int RoleId { get; set; }

        public string RoleDesc { get; set; }

        public bool Active { get; set; }

        public List<int> ChurchIds { get; set; }
    }
}