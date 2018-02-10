namespace NtccSteward.Core.Models.Message
{
    public interface IRecipient
    {
        int Id { get; set; }
        int ContactInfoId { get; set; }
        int IdentityId { get; set; }
        int MessageRecipientGroupId { get; set; }
        string Name { get; set; }
        string Address { get; set; } // email or phone number
    }
}