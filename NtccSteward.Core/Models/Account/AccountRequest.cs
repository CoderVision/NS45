using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Models.Account
{
    public class AccountRequest : IAccountRequest
    {
        public AccountRequest()
        {

        }

        public AccountRequest(IAccountRequest ar)
        {
            FirstName = ar.FirstName;
            LastName = ar.LastName;
            Email = ar.Email;
            Password = ar.Password;
            PastorName = ar.PastorName;
            ChurchName = ar.ChurchName;
            City = ar.City;
            State = ar.State;
            Comments = ar.Comments;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PastorName { get; set; }
        public string ChurchName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Comments { get; set; }
    }
}
