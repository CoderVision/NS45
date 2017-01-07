using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Interfaces.Account
{
    public interface ILogin
    {
        string Email { get; set; }
        string Password { get; set; }
        int ChurchId { get; set; }
    }
}
