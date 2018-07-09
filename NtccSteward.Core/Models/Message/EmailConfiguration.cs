using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Message
{
    public class EmailConfiguration
    {
        public int Id { get; set; }

        public int ChurchId { get; set; }

        public int EmailConfigProfileId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}