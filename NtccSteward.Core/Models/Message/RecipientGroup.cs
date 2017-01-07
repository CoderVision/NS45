using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class RecipientGroup : IRecipientGroup
    {
        public RecipientGroup() { }

        public int ID { get; set; } // MessageRecipientGroupId

        public string Name { get; set; }

        public string Description { get; set; }

        public int ChurchId { get; set; }
    }
}
