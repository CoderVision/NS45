using NtccSteward.Core.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NtccSteward.Core.Models.Account
{
    public class Session : ISession
    {
        public Session()
        {
            Roles = new List<IRole>();
        }

        public int UserId { get; set; }

        public int ChurchId { get; set; }

        public string SessionId { get; set; }

        public List<IRole> Roles { get; set; }
    }
}
