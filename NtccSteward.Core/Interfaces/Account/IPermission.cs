namespace NtccSteward.Core.Interfaces.Account
{
    public interface IPermission
    {
        string PermissionDesc { get; set; }
        int PermissionID { get; set; }
        int Value { get; set; }
    }
}