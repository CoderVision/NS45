using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Common.Parameters
{
    public class GetMessagesParameter
    {
        public int RecipientGroupId { get; set; }  // this is a problem.  Should not be Member ID, but correspondence id or identity id 

        public int MaxReturnRows { get; set; }
    }
}
