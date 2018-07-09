using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Import
{
    public class Soulwinner
    {
        public int SoulwinnerId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Generation { get; set; } // II, III, Jr, Sr.

        public bool IsHere { get; set; }  // indicates the person is at this location

        public string Gender { get; set; }

        public bool IsMinister { get; set; }

        public bool IsLayPastor { get; set; }

        public bool IsActive { get; set; }

        public string MarriedStatus { get; set; }  // (M)arried, (O)ther or (S)ingle

        public string SpouseName { get; set; }

        public bool IsSpousePartner { get; set; }  // Spouse Partner?

        public int LayPastorId { get; set; }  // SWLPTR

        public bool IsDoorToDoor { get; set; }

        public bool IsCasualStatus { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int IdentityId { get; set; }  // Member Identity Id
    }
}