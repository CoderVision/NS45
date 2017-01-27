using System.Collections.Generic;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Interfaces.Common.Address;
using System;

namespace NtccSteward.Core.Interfaces.Members
{
    public interface IMemberProfile
    {
        //List<Address> AddressList { get; set; }
        DateTime? BirthDate { get; set; }
        int ChurchId { get; set; }
        string ChurchName { get; set; }
        string Comments { get; set; }
        string DateBaptizedHolyGhost { get; set; }
        string DateBaptizedWater { get; set; }
        string DateSaved { get; set; }
        //List<Email> EmailList { get; set; }
        string FirstName { get; set; }
        string FullName { get; }
        string Gender { get; set; }
        string LastName { get; set; }
        bool Married { get; set; }
        int MemberId { get; set; }
        int MemberStatusEnumType { get; set; }
        string MiddleName { get; set; }
        //List<Phone> PhoneList { get; set; }
        string PreferredName { get; set; }
        string Sponsor { get; set; }
        int SponsorId { get; set; }
        int StatusId { get; set; }
        string StatusDesc { get; set; }
        int StatusChangeTypeId { get; set; }
        string StatusChangeTypeDesc { get; set; }

        bool Veteran { get; set; }
    }
}