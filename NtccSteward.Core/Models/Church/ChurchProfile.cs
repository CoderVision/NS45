﻿using NtccSteward.Core.Interfaces.Church;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NtccSteward.Core.Models.Team;

namespace NtccSteward.Core.Models.Church
{
    public class ChurchProfile : IChurchProfile
    {
        public ChurchProfile()
        {
            AddressList = new List<Address>();
            EmailList = new List<Email>();
            PhoneList = new List<Phone>();
            CustomAttributeList = new List<CustomAttribute>();
            MetaDataList = new List<AppEnum>();
        }
        public List<Address> AddressList { get; set; }

        public List<Email> EmailList { get; set; }

        public List<Phone> PhoneList { get; set; }

        public List<CustomAttribute> CustomAttributeList { get; set; }

        public List<AppEnum> MetaDataList { get; set; }

        public int Id { get; set; } 

        public string Name { get; set; }

        public int StatusId { get; set; }

        public string StatusDesc { get; set; }

        public string Comment { get; set; }

        public Team.Team PastoralTeam { get; set; }
    }
}