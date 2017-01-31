using NtccSteward.Core.Interfaces.Members;
using NtccSteward.Core.Models.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Factories
{
    public class MemberFactory
    {
        public T CreateMemberProfile<T>(IMemberProfile source)
            where T : IMemberProfile, new()
        {
            var target = new T();
            target.MemberId = source.MemberId;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.MiddleName = source.MiddleName;
            target.Gender = source.Gender;
            target.SponsorId = source.SponsorId;
            target.Sponsor = source.Sponsor;
            target.StatusId = source.StatusId;
            target.StatusDesc = source.StatusDesc;
            target.BirthDate = source.BirthDate;
            target.ChurchId = source.ChurchId;
            target.ChurchName = source.ChurchName;
            target.Comments = source.Comments;
            target.PreferredName = source.PreferredName;
            target.DateSaved = source.DateSaved;
            target.DateBaptizedWater = source.DateBaptizedWater;
            target.DateBaptizedHolyGhost = source.DateBaptizedHolyGhost;
            target.Married = source.Married;
            target.Veteran = source.Veteran;
            target.StatusChangeTypeId = source.StatusChangeTypeId;
            target.StatusChangeTypeDesc = source.StatusChangeTypeDesc;
            target.MemberTypeEnumId = source.MemberTypeEnumId;

            return target;
        }
    }
}
