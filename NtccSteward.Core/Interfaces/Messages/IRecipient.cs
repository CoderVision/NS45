namespace NtccSteward.Core.Models.Message
{
    public interface IRecipient
    {
        string Address { get; set; }
        int ContactInfoId { get; set; }
        int IdentityId { get; set; }
        string Name { get; set; }
    }
}