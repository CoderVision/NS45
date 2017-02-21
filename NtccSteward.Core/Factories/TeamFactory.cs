using NtccSteward.Core.Interfaces.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Core.Factories
{
    public class TeamFactory
    {
        public T CreateTeam<T>(ITeam source) where T : ITeam, new()
        {
            var t = new T();
            t.Id = source.Id;
            t.Name = source.Name;
            t.ChurchId = source.ChurchId;
            t.TeamTypeEnumId = source.TeamTypeEnumId;
            t.TeamPositionEnumTypeId = source.TeamPositionEnumTypeId;
            return t;
        }
    }
}