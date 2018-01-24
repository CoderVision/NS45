using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Members
{
    public class Team
    {
        public int MemberId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
}