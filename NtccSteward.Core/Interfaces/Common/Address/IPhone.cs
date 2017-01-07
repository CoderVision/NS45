namespace NtccSteward.Core.Interfaces.Common.Address
{
    public interface IPhone : IAddressInfo
    {
        string PhoneNumber { get; set; }
        int PhoneType { get; set; }
    }
}