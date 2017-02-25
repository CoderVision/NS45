using NtccSteward.Core.Interfaces.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Team
{
    public class Team : ITeam
    {
        public Team()
        {
            Teammates = new List<Teammate>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public int ChurchId { get; set; }

        /// <summary>
        /// Pastoral Team, Evangelistic Team, etc.
        /// </summary>
        public int TeamTypeEnumId { get; set; }

        public string TeamTypeEnumDesc { get; set; }

        /// <summary>
        /// Pastor, Assoc., Minister, Helper, etc.
        /// </summary>
        public int TeamPositionEnumTypeId { get; set; }

        public List<Teammate> Teammates { get; set; }
    }
}