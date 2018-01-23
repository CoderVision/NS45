namespace NtccSteward.Core.Interfaces.Account
{
    public interface IAccountRequest
    {
        int ChurchId { get; set; }
        string ChurchName { get; set; }
        string City { get; set; }
        string Comments { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Password { get; set; }
        string PastorName { get; set; }
        string State { get; set; }
        int RequestId { get; set; }
        string Line1 { get; set; }
        string Zip { get; set; }
    }
}