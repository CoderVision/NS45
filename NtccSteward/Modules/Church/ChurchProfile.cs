﻿using NtccSteward.Core.Interfaces.Church;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.ViewModels;
using NtccSteward.ViewModels.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchProfile : ModuleBase, IChurchProfile
    {
        public ChurchProfile()
        {
            Initialize();
        }

        private void Initialize()
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

        public int id { get; set; } // specific name convention necessray for SlickGrid

        public string Name { get; set; }

        public int StatusId { get; set; }

        public string StatusDesc { get; set; }

        public int PastorId { get; set; }

        public string Pastor { get; set; }

        public string Comment { get; set; }

        public TeamViewModel PastoralTeam { get; set; }
    }
}
