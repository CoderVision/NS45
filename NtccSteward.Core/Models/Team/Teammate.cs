using NtccSteward.Core.Interfaces.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Models.Team
{
    public class Teammate : ITeammate
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int PersonId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Pastor, Assoc., Helper, etc.
        /// </summary>
        public int TeamPositionEnumId { get; set; }
    }
}