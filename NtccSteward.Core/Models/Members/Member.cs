using NtccSteward.Core.Interfaces.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Members
{
    public class Member : IMember
    {
        public int id { get; set; } // specific name convention necessary for SlickGrid

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return (FirstName + " " + LastName);
            }
        }

        public string Status { get; set; }

        public string StatusChangeType { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ActivityDate { get; set; }
    }
}