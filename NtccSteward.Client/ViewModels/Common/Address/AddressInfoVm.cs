using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Common.Address
{
    public class AddressInfoVm : IAddressInfo
    {
        public AddressInfoVm()
        { }

        public AddressInfoVm(IAddressInfo addyInfo)
        {
            this.ContactInfoId = addyInfo.ContactInfoId;
            this.IdentityId = addyInfo.IdentityId;
            this.Preferred = addyInfo.Preferred;
            this.Verified = addyInfo.Verified;
            this.Changed = false;
        }

        public int ContactInfoLocationType { get; set; }

        public int ContactInfoType { get; set; }

        public int Id { get; set; }

        public int IdentityId { get; set; }

        public bool Preferred { get; set; }

        public bool Verified { get; set; }

        public bool Changed { get; set; }

        public int ContactInfoId
        {
            get { return Id; }
            set { Id = value; }
        }

        public int ModifiedByIdentityId { get; set; }

        public string Note { get; set; }

        public bool Archived { get; set; }
    }
}
