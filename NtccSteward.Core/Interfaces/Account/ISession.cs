using NtccSteward.Core.Models.Account;
using System;
using System.Collections.Generic;

namespace NtccSteward.Core.Interfaces.Account
{
    public interface ISession
    {
        int UserId { get; set; }
        List<Role> Roles { get; set; }
        string SessionId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        bool Active { get; set; }
    }
}