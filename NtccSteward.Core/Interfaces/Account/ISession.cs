using NtccSteward.Core.Models.Account;
using System;
using System.Collections.Generic;

namespace NtccSteward.Core.Interfaces.Account
{
    public interface ISession
    {
        int UserId { get; set; }
        int ChurchId { get; set; }
        List<Role> Roles { get; set; }
        string SessionId { get; set; }
    }
}