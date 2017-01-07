namespace NtccSteward.Core.Interfaces.Common.Address
{
    public interface IEmail : IAddressInfo
    {
        string EmailAddress { get; set; }
    }
}