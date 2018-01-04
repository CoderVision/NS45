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

        public int StatusId { get; set; }

        public string StatusChangeType { get; set; }

        public int StatusChangeTypeId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        // last activity date
        public DateTimeOffset? ActivityDate { get; set; }

        public string Comments { get; set; }

        public string Sponsor { get; set; }

        public int SponsorId { get; set; }

        public int TeamId { get; set; }
        public string Team { get; set; }
    }
}