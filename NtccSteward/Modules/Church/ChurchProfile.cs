using NtccSteward.ViewModels.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Modules.Church
{
    public class ChurchProfile : ModuleBase
    {
            public ChurchProfile()
            {
                Initialize();
            }

            public ChurchProfile(int id)
            {
                this.Id = id;

                Initialize();
            }

            //public override void Load()
            //{
            //    this.ChurchName = "NTCC Graham, Wa.";

            //    // load addresses from database, but don't add an empty one.

            //    Initialize();
            
            //    Addys.Add(new AddressVm() { Line1 = "13304 210th St. E", City = "Graham", Id = 1 }); // add empty addy
            //    EmailAddys.Add(new EmailVm() { EmailAddress = "ntccofa@gmail.com", Id = 1 }); // add empty addy
            //    PhoneNums.Add(new PhoneVm() { AreaCode = 360, Prefix = 893, LineNumber = 3258, Id = 1 }); // add empty addy
            //}

            private void Initialize()
            {
                this.DisplayText = "Profile";

                Addys = new List<AddressVm>();
                EmailAddys = new List<EmailVm>();
                PhoneNums = new List<PhoneVm>();
            }

            public string ChurchName { get; set; }

            public string Comments { get; set; }

            public List<AddressVm> Addys { get; set; }

            public List<EmailVm> EmailAddys { get; set; }

            public List<PhoneVm> PhoneNums { get; set; }
    }
}
