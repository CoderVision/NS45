using NtccSteward.Core.Models.Account;
using System.Collections.Generic;

namespace NtccSteward.Core.Interfaces.Account
{
    public interface IRole
    {
        List<Permission> Permissions { get; set; }
        string RoleDesc { get; set; }
        int RoleId { get; set; }
    }
}