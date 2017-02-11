using NtccSteward.Core.Models.Team;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository
{
    public interface ITeamRepository
    {
        List<Team> GetList(int churchId, int? teamId = null);
        List<Teammate> GetTeammates(int teamId);
    }

    public class TeamRepository : NtccSteward.Repository.Repository, ITeamRepository
    {
        private readonly SqlCmdExecutor _executor;

        public TeamRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
        }

        public List<Team> GetList(int churchId, int? teamId = null)
        {
            var list = new List<Team>();

            var proc = "GetTeams";
            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("churchId", churchId));

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var team = new Team();
                        team.Id = (int)reader["TeamId"];
                        team.Name = reader.ValueOrDefault<string>("Name", string.Empty);
                        team.ChurchId = (int)reader["ChurchId"];
                        team.TeamTypeEnumId = (int)reader["TeamTypeEnumId"];
                        team.TeamPositionEnumTypeId = (int)reader["TeamPositionEnumTypeId"];
                        list.Add(team);
                    }
                }
            }

            return list;
        }


        public List<Teammate> GetTeammates(int teamId)
        {
            var list = new List<Teammate>();

            var proc = "GetTeammates";
            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("teamId", teamId));

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var team = list.First(t => t.Id == (int)reader["TeamId"]);

                        var teammate = new Teammate();
                        teammate.Id = (int)reader["TeammateId"];
                        teammate.TeamId = (int)reader["TeamId"];
                        teammate.PersonId = (int)reader["PersonId"];
                        teammate.TeamPositionEnumId = (int)reader["TeamPositionEnumId"];
                        teammate.Name = reader.ValueOrDefault<string>("Name", string.Empty);
                        list.Add(teammate);
                    }
                }
            }

            return list;
        }
    }
}