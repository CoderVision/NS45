using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Address
{
    public class AddressInfo : IAddressInfo
    {
        public int ContactInfoId { get; set; }

        public int IdentityId { get; set; }

        public bool Preferred { get; set; }

        public bool Verified { get; set; }

        public int ContactInfoType { get; set; }

        public int ContactInfoLocationType { get; set; }

        public int ModifiedByIdentityId { get; set; }

        public string Note { get; set; }

        public bool Archived { get; set; }
    }
}
