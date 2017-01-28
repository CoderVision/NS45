using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Common.Address
{
    public class PhoneVm : AddressInfoVm, IPhone
    {
        public PhoneVm()
        { }

        public PhoneVm(IPhone phone)
            : base(phone)
        {
            this.PhoneNumber = phone.PhoneNumber;
        }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name ="Phone")]
        public string PhoneNumber { get; set; }

        public int PhoneType { get; set; }
    }
}
