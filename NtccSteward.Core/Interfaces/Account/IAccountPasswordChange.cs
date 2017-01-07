namespace NtccSteward.Core.Interfaces.Account
{
    public interface IAccountPasswordChange
    {
        string Email { get; set; }
        string NewPassword { get; set; }
        string OldPassword { get; set; }
    }
}