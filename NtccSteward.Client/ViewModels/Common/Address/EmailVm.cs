using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.ViewModels.Common.Address
{
    public class EmailVm : AddressInfoVm, IEmail
    {
        public EmailVm()
        { }

        public EmailVm(IEmail email)
            : base(email)
        {
            this.EmailAddress = email.EmailAddress;
        }

        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
