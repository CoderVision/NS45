using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Account
{
    public class Permission : IPermission
    {
        public int PermissionID { get; set; }

        public string PermissionDesc { get; set; }

        public int Value { get; set; }
    }
}
