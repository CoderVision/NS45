using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Message
{
    public class RecipientGroup : IRecipientGroup
    {
        public RecipientGroup() { }

        public int Id { get; set; } // MessageRecipientGroupId

        public string Name { get; set; }

        public string Description { get; set; }

        public int ChurchId { get; set; }

        public List<Recipient> Recipients { get; set; }

        public List<Message> Messages { get; set; }

        public DateTimeOffset LastMessageDate { get; set; }

        public string LastMessageSubject { get; set; }

        public string LastMessageBody { get; set; }
    }
}
