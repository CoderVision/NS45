using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Account
{
    public class Login : ILogin
    {
        public Login() { }

        public Login(ILogin login)
        {
            Email = login.Email;
            Password = login.Password;
            ChurchId = login.ChurchId;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int ChurchId { get; set; }  // user will have to select a church id
    }
}

