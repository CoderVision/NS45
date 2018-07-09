using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class Recipient : IRecipient
    {
        public int Id { get; set; } // Member ID or Church ID

        public string Name { get; set; }  // name of recipient

        public int ContactInfoId { get; set; }  // id of the phone or email to use

        public string Address { get; set; } // phone number or email address

        public int IdentityId { get; set; }
        public int MessageRecipientGroupId { get; set; }

        public bool PreferredAddress { get; set; }
    }
}
