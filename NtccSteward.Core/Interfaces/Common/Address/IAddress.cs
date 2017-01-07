namespace NtccSteward.Core.Interfaces.Common.Address
{
    public interface IAddress : IAddressInfo
    {
        string City { get; set; }
        string Line1 { get; set; }
        string Line2 { get; set; }
        string Line3 { get; set; }
        string State { get; set; }
        string Zip { get; set; }
    }
}