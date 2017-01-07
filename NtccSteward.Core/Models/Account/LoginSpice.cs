using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Account
{
    public class LoginSpice
    {
        public int PersonIdentityID { get; set; }

        public string Salt { get; set; }
    }
}
