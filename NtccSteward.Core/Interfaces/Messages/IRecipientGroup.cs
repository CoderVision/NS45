using System;
using System.Collections.Generic;

namespace NtccSteward.Core.Models.Message
{
    public interface IRecipientGroup
    {
        int ChurchId { get; set; }
        string Description { get; set; }
        int Id { get; set; }
        int MessageTypeEnumID { get; set; }
        string Name { get; set; }
        List<Message> Messages { get; set; }
        List<Recipient> Recipients { get; set; }
        DateTimeOffset LastMessageDate { get; set; }
        string LastMessageSubject { get; set; }
        string LastMessageBody { get; set; }
    }
}