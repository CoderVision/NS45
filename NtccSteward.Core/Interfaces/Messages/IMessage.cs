using System;

namespace NtccSteward.Core.Interfaces.Messages
{
    public interface IMessage
    {
        long MessageID { get; set; }
        DateTime MessageDate { get; set; }
        int IdentityID { get; set; }
        string IdentityName { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        int MessageTypeEnumID { get; set; }
        int MessageDirectionEnumID { get; set; }
    }
}