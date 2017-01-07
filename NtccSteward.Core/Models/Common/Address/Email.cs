using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Address
{
    public class Email : AddressInfo, IEmail
    {
        public string EmailAddress { get; set; }
    }
}
