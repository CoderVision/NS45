using ca = NtccSteward.Core.Models.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Interfaces.Common.Address;
using System.ComponentModel.DataAnnotations;

namespace NtccSteward.ViewModels.Common.Address
{
    public class AddressVm : AddressInfoVm, IAddress
    {
        public AddressVm()
        {
        }

        public AddressVm(IAddress addy)
            : base(addy)
        {
            Line1 = addy.Line1;
            Line2 = addy.Line2;
            Line3 = addy.Line3;
            City = addy.City;
            State = addy.State;
            Zip = addy.Zip;
        }
        
        [MaxLength(50)]
        [Display(Name ="Address")]
        public string Line1 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        [MaxLength(50)]
        [Display(Name = "City")]
        public string City { get; set; }

        [MaxLength(2)]
        [Display(Name = "St")]
        public string State { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip code")]
        public string Zip { get; set; }
    }
}
