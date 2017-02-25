using NtccSteward.Core.Interfaces.Team;
using NtccSteward.Core.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.ViewModels.Team
{
    public class TeamViewModel : ITeam
    {
        public TeamViewModel()
        {
            Teammates = new List<Teammate>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public int ChurchId { get; set; }

        /// <summary>
        /// Pastoral Team, Evangelistic Team, etc.
        /// </summary>
        public int TeamTypeEnumId { get; set; }

        /// <summary>
        /// Pastor, Assoc., Minister, Helper, etc.
        /// </summary>
        public int TeamPositionEnumTypeId { get; set; }

        public List<Teammate> Teammates { get; set; }
    }
}