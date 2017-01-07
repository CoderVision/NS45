namespace NtccSteward.Core.Interfaces.Common.Address
{
    public interface IAddressInfo
    {
        int ContactInfoId { get; set; }
        int IdentityId { get; set; }
        bool Preferred { get; set; }
        bool Verified { get; set; }
        int ContactInfoType { get; set; }
        int ContactInfoLocationType { get; set; }
        int ModifiedByIdentityId { get; set; }
        string Note { get; set; }
        bool Archived { get; set; }
    }
}