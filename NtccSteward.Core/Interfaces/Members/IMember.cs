namespace NtccSteward.Core.Interfaces.Members
{
    public interface IMember
    {
        string FirstName { get; set; }
        string FullName { get; }
        int id { get; set; }
        string LastName { get; set; }
        string Status { get; set; }
        string StatusChangeType { get; set; }
    }
}