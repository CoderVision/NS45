using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Interfaces.Members;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.ViewModels.Common.Address;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace NtccSteward.Modules.Members
{
    public class MemberProfile : ModuleBase, IMemberProfile
    {
        public MemberProfile()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.DisplayText = "Profile";

            AddressList = new List<AddressVm>();
            EmailList = new List<EmailVm>();
            PhoneList = new List<PhoneVm>();
            CustomAttributeList = new List<CustomAttribute>();
            MetaDataList = new List<AppEnum>();
        }

        private void SetFullName()
        {
            var firstName = !string.IsNullOrWhiteSpace(FirstName) ? FirstName + " " : string.Empty;

            var middleName = !string.IsNullOrWhiteSpace(MiddleName) ? MiddleName + " " : string.Empty;

            var lastName = LastName?.Trim();

            FullName = $"{firstName}{middleName}{lastName}";
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                SetFullName();
            }
        }

        private string _middleName;
        public string MiddleName
        {
            get
            {
                return _middleName;
            }
            set
            {
                _middleName = value;
                SetFullName();
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                SetFullName();
            }
        }

        public string FullName { get; private set; }

        public string Gender { get; set; }

        //[DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string PreferredName { get; set; }

        public string Comments { get; set; }

        public List<AddressVm> AddressList { get; set; }

        public List<EmailVm> EmailList { get; set; }

        public List<PhoneVm> PhoneList { get; set; }

        public List<CustomAttribute> CustomAttributeList { get; set; }

        public List<AppEnum> MetaDataList { get; set; }

        public string ChurchName { get; set; }

        public int ChurchId { get; set; }

        public int StatusId { get; set; } // faithful, etc.  

        public string StatusDesc { get; set; }

        public int StatusChangeTypeId { get; set; }

        public string StatusChangeTypeDesc { get; set; }

        public int SponsorId { get; set; }

        public string Sponsor { get; set; }

        public string DateSaved { get; set; }

        public string DateBaptizedHolyGhost { get; set; }

        public string DateBaptizedWater { get; set; }

        public int MemberStatusEnumType { get; set; }

        public bool Married { get; set; }

        public bool Veteran { get; set; }

        private int memberId;
        public int MemberId
        {
            get { return memberId; }
            set
            {
                memberId = value;
                base.Id = value;
            }
        }
    }
}
