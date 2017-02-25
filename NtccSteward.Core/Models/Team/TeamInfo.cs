using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Team
{
    public class TeamInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public int TeamTypeEnumId { get; set; }

        public int TeamPositionEnumTypeId { get; set; }

        public int ChurchId { get; set; }
    }
}