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
        Team GetTeam(int teamId);
        List<Teammate> GetTeammates(int teamId);
        RepositoryActionResult<Teammate> DeleteTeammate(int teamId, int teammateId);
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


        public Team GetTeam(int teamId)
        {
            Team team = null;

            var proc = "GetTeam";
            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("teamId", @teamId));

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    int pteamId = 0;
                    while (reader.Read())
                    {
                        if (pteamId != (int)reader["TeamId"])
                        {
                            pteamId = (int)reader["TeamId"];

                            team = new Team();
                            team.Id = (int)reader["TeamId"];
                            team.Name = reader.ValueOrDefault<string>("Name", string.Empty);
                            team.ChurchId = (int)reader["ChurchId"];
                            team.TeamTypeEnumId = (int)reader["TeamTypeEnumId"];
                            team.TeamPositionEnumTypeId = (int)reader["TeamPositionEnumTypeId"];
                        }

                        var idx = reader.GetOrdinal("TeammateId");
                        if (!reader.IsDBNull(idx))
                        {
                            // add teammates
                            var teammate = new Teammate();
                            teammate.Id = (int)reader["TeammateId"];
                            teammate.TeamId = (int)reader["TeamId"];
                            teammate.PersonId = (int)reader["PersonId"];
                            teammate.TeamPositionEnumId = (int)reader["TeamPositionEnumId"];
                            teammate.Name = reader.ValueOrDefault<string>("TeammateName", string.Empty);
                            team.Teammates.Add(teammate);
                        }
                    }
                }
            }

            return team;
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


        public RepositoryActionResult<Teammate> DeleteTeammate(int teamId, int teammateId)
        {
            try
            {
                var paramz = new List<SqlParameter>();
                paramz.Add(new SqlParameter("teamId", teamId));
                paramz.Add(new SqlParameter("teammateId", teammateId));

                Func<SqlDataReader, int> readFx = (reader) =>
                {
                    return (int)reader["id"];
                };

                var list = _executor.ExecuteSql<int>("DeleteTeammate", CommandType.StoredProcedure, paramz, readFx);

                if (list != null && list.Any())
                {
                    int id = (int)list.FirstOrDefault();
                    return new RepositoryActionResult<Teammate>(null, RepositoryActionStatus.Deleted);
                }

                return new RepositoryActionResult<Teammate>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Teammate>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}