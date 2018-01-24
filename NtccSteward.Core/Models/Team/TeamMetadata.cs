using NtccSteward.Core.Models.Common.Enums;
using c = NtccSteward.Core.Models.Church;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Team
{
    public class TeamMetadata
    {
        public TeamMetadata()
        {
            Enums = new List<AppEnum>();
            EnumTypes = new List<AppEnum>();
            Churches = new List<c.Church>();
            Members = new List<Models.Members.Member>();
        }
        public List<AppEnum> EnumTypes { get; set; }
        public List<AppEnum> Enums { get; set; }
        public List<c.Church> Churches { get; set; }
        public List<Members.Member> Members { get; set; }
    }
}