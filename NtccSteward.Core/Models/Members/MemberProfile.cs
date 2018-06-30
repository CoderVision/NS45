using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Interfaces.Members;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Members
{
    public class MemberProfile : IMemberProfile
    {
        public MemberProfile()
        {
            AddressList = new List<Address>();
            EmailList = new List<Email>();
            PhoneList = new List<Phone>();
            CustomAttributeList = new List<CustomAttribute>();
            TeamList = new List<Team>();
            SponsorList = new List<Sponsor>();
        }

        public int MemberId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public string PreferredName { get; set; }

        public string Comments { get; set; }

        public List<Address> AddressList { get; set; }

        public List<Email> EmailList { get; set; }

        public List<Phone> PhoneList { get; set; }

        public List<CustomAttribute> CustomAttributeList { get; set; }

        public List<Team> TeamList { get; set; }

        public List<Sponsor> SponsorList { get; set; }

        public int ChurchId { get; set; }

        public string ChurchName { get; set; }

        public int StatusId { get; set; } // faithful, etc.  

        public string StatusDesc { get; set; }

        public int StatusChangeTypeId { get; set; }

        public string StatusChangeTypeDesc { get; set; }

        public int SponsorId { get; set; }

        public string Sponsor { get; set; }

        public bool Married { get; set; }

        public bool Veteran { get; set; }

        public DateTime? DateSaved { get; set; }

        public DateTime? DateBaptizedHolyGhost { get; set; }

        public DateTime? DateBaptizedWater { get; set; }

        public int MemberStatusEnumType { get; set; }

        public int MemberTypeEnumId { get; set; }

        public string Suffix { get; set; }

        public bool IsHere { get; set; }

        public int LanguageTypeEnumId { get; set; }

        public bool HasBeenBaptized { get; set; }
    }
}
