using NtccSteward.Core.Models.Common.Enums;
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
        }
        public List<AppEnum> EnumTypes { get; set; }
        public List<AppEnum> Enums { get; set; }
    }
}