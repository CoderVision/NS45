using NtccSteward.Core.Models.Account;
using System.Collections.Generic;

namespace NtccSteward.Core.Interfaces.Account
{
    public interface IRole
    {
        List<IPermission> Permissions { get; set; }
        string RoleDesc { get; set; }
        int RoleID { get; set; }
    }
}