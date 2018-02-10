using System;

namespace NtccSteward.Core.Interfaces.Messages
{
    public interface IMessage
    {
        long Id { get; set; }
        int RecipientId { get; set; }
        DateTimeOffset MessageDate { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        int MessageTypeEnumID { get; set; }
        int MessageDirectionEnumID { get; set; }
    }
}