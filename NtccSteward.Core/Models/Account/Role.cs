using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Account
{
    public class Role : IRole
    {
        public Role()
        {
            Permissions = new List<IPermission>();
        }

        public int RoleID { get; set; }

        public string RoleDesc { get; set; }

        public List<IPermission> Permissions { get; set; }
    }
}
