using System.Collections.Generic;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Interfaces.Team;
using ct = NtccSteward.Core.Models.Team;

namespace NtccSteward.Core.Interfaces.Church
{
    public interface IChurchProfile
    {
        List<Address> AddressList { get; set; }

        List<Email> EmailList { get; set; }

        List<Phone> PhoneList { get; set; }

        List<CustomAttribute> CustomAttributeList { get; set; }

        List<AppEnum> MetaDataList { get; set; }

        int Id { get; set; } 

        string Name { get; set; }

        int StatusId { get; set; }

        string StatusDesc { get; set; }

        string Comment { get; set; }

        ct.Team PastoralTeam { get; set; }
    }
}