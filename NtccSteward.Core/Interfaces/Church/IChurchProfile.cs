using System.Collections.Generic;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;

namespace NtccSteward.Core.Interfaces.Church
{
    public interface IChurchProfile
    {
        List<Address> AddressList { get; set; }

        List<Email> EmailList { get; set; }

        List<Phone> PhoneList { get; set; }

        List<CustomAttribute> CustomAttributeList { get; set; }

        List<AppEnum> MetaDataList { get; set; }

        int id { get; set; } // specific name convention necessray for SlickGrid

        string Name { get; set; }

        int StatusId { get; set; }

        string StatusDesc { get; set; }

        int PastorId { get; set; }

        string Pastor { get; set; }

        string Comment { get; set; }
    }
}