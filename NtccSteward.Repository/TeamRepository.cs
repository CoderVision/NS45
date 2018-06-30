using NtccSteward.Core.Interfaces.Team;
using NtccSteward.Core.Models.Common.Enums;
//using NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Team;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using c = NtccSteward.Core.Models.Church;

namespace NtccSteward.Repository
{
    public interface ITeamRepository
    {
        List<Core.Models.Team.Team> GetList(int churchId);
        Team GetTeam(int teamId);
        RepositoryActionResult<ITeam> SaveTeam(ITeam Team);

        RepositoryActionResult<Team> DeleteTeam(int id);

        List<Teammate> GetTeammates(int teamId);
        RepositoryActionResult<Teammate> SaveTeammate(Teammate teammate);
        RepositoryActionResult<Teammate> DeleteTeammate(int teamId, int teammateId);

        RepositoryActionResult<TeamMetadata> GetMetadata(int churchId, int userId);
    }

    public class TeamRepository : NtccSteward.Repository.Repository, ITeamRepository
    {
        private readonly SqlCmdExecutor _executor;

        public TeamRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
        }

        public RepositoryActionResult<TeamMetadata> GetMetadata(int churchId, int userId)
        {
            var metadata = new TeamMetadata();

            using (var cn = new SqlConnection(_executor.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetTeamProfileMetadata", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("churchId", churchId));
                    cmd.Parameters.Add(new SqlParameter("userId", userId));

                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // read enum types
                            while (reader.Read())
                            {
                                var appEnum = new AppEnum();
                                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");
                                appEnum.OptionsEnumTypeID = reader.ValueOrDefault<int>("OptionsEnumTypeID");

                                metadata.EnumTypes.Add(appEnum);
                            }

                            // read enums for above enum types
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var appEnum = new AppEnum();
                                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");
                                appEnum.OptionsEnumTypeID = reader.ValueOrDefault<int>("OptionsEnumTypeID");

                                metadata.Enums.Add(appEnum);
                            }

                            // read churches
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var church = new c.Church();
                                church.id = reader.ValueOrDefault<int>("Id");
                                church.Name = reader.ValueOrDefault<string>("Name");

                                metadata.Churches.Add(church);
                            }

                            // read members
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var member = new Core.Models.Members.Member();
                                member.id = reader.ValueOrDefault<int>("Id");
                                member.FirstName = reader.ValueOrDefault<string>("FirstName");
                                member.LastName = reader.ValueOrDefault<string>("LastName");

                                metadata.Members.Add(member);
                            }
                        }
                    }
                }
            }

            return new RepositoryActionResult<TeamMetadata>(metadata, RepositoryActionStatus.Ok);
        }

        public List<Team> GetList(int churchId)
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
                        team.Desc = reader.ValueOrDefault<string>("Desc", string.Empty);
                        team.ChurchId = (int)reader["ChurchId"];
                        team.TeamTypeEnumId = (int)reader["TeamTypeEnumId"];
                        team.TeamTypeEnumDesc = reader["TeamTypeEnumDesc"].ToString();
                        team.TeamPositionEnumTypeId = (int)reader["TeamPositionEnumTypeId"];
                        team.TeammateCount = (int)reader["TeammateCount"];
                        team.TeamLeader = reader.ValueOrDefault<string>("TeamLeader", string.Empty);

                        list.Add(team);
                    }
                }
            }

            return list;
        }

        public RepositoryActionResult<ITeam> SaveTeam(ITeam team)
        {
            var proc = "SaveTeam";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("id", team.Id));
            paramz.Add(new SqlParameter("name", team.Name));
            paramz.Add(new SqlParameter("desc", team.Desc));
            paramz.Add(new SqlParameter("churchId", team.ChurchId));
            paramz.Add(new SqlParameter("teamTypeEnumId", team.TeamTypeEnumId));
            paramz.Add(new SqlParameter("comment", team.Comment));

            Func<SqlDataReader, Tuple<int,int>> readFx = (reader) =>
            {
                var id = (int)reader["Id"];
                var teamPositionEnumTypeId = (int)reader["TeamPositionEnumTypeId"];

                return new Tuple<int, int>(id, teamPositionEnumTypeId);
            };

            var list = _executor.ExecuteSql<Tuple<int, int>>(proc, CommandType.StoredProcedure, paramz, readFx);

            var newTeam = list.FirstOrDefault();
            if (newTeam.Item1 > 0)
            {
                // update id and position type
                team.Id = newTeam.Item1;
                team.TeamPositionEnumTypeId = newTeam.Item2;

                return new RepositoryActionResult<ITeam>(team, RepositoryActionStatus.Created);
            }
            else
            {
                return new RepositoryActionResult<ITeam>(null, RepositoryActionStatus.Error);
            }
        }

        public Team GetTeam(int teamId)
        {
            Team team = null;

            var proc = "GetTeam";
            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("teamId", teamId));

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
                            team.Desc = reader.ValueOrDefault<string>("Desc", string.Empty);
                            team.ChurchId = (int)reader["ChurchId"];
                            team.TeamTypeEnumId = (int)reader["TeamTypeEnumId"];
                            team.TeamPositionEnumTypeId = (int)reader["TeamPositionEnumTypeId"];
                            team.Comment = reader.ValueOrDefault<string>("Comment", string.Empty);
                        }

                        var idx = reader.GetOrdinal("TeammateId");
                        if (!reader.IsDBNull(idx))
                        {
                            // add teammates
                            var teammate = new Teammate();
                            teammate.Id = (int)reader["TeammateId"];
                            teammate.TeamId = (int)reader["TeamId"];
                            teammate.MemberId = (int)reader["EntityId"];
                            teammate.TeamPositionEnumId = (int)reader["TeamPositionEnumId"];
                            teammate.TeamStatusTypeEnumId = reader.ValueOrDefault("TeamStatusTypeEnumId", 0);
                            teammate.TeamPositionEnumDesc = reader.ValueOrDefault<string>("Position", string.Empty);
                            teammate.Name = reader.ValueOrDefault<string>("TeammateName", string.Empty);
                            team.Teammates.Add(teammate);
                        }
                    }
                }
            }

            return team;
        }

        public RepositoryActionResult<Team> DeleteTeam(int teamId)
        {
            var proc = "DeleteTeam";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("teamId", @teamId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["Id"];
            };

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            var id = list.FirstOrDefault();
            if (id > 0)
            {
                return new RepositoryActionResult<Team>(null, RepositoryActionStatus.Deleted);
            }
            else
            {
                return new RepositoryActionResult<Team>(null, RepositoryActionStatus.NotFound);
            }
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
                        teammate.MemberId = (int)reader["EntityId"];
                        teammate.TeamPositionEnumId = (int)reader["TeamPositionEnumId"];
                        teammate.TeamStatusTypeEnumId = reader.ValueOrDefault("TeamStatusTypeEnumId", 0);
                        teammate.TeamPositionEnumDesc = reader.ValueOrDefault<string>("TeamPositionEnumDesc", string.Empty);
                        teammate.Name = reader.ValueOrDefault<string>("Name", string.Empty);
                        teammate.ChurchId = (int)reader["ChurchId"];

                        list.Add(teammate);
                    }
                }
            }

            return list;
        }


        public RepositoryActionResult<Teammate> DeleteTeammate(int teamId, int memberId)
        {
            try
            {
                var paramz = new List<SqlParameter>();
                paramz.Add(new SqlParameter("teamId", teamId));
                paramz.Add(new SqlParameter("memberId", memberId));

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

        public RepositoryActionResult<Teammate> SaveTeammate(Teammate teammate)
        {
            var proc = "SaveTeammate";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("teammateId", teammate.Id));
            paramz.Add(new SqlParameter("teamId", teammate.TeamId));
            paramz.Add(new SqlParameter("entityId", teammate.MemberId));
            paramz.Add(new SqlParameter("entityTypeEnumId", 56)); // 56:  entity is a person.  could be a team (78), or something else
            paramz.Add(new SqlParameter("teamPositionEnumId", teammate.TeamPositionEnumId));
            paramz.Add(new SqlParameter("teamStatusTypeEnumId", teammate.TeamStatusTypeEnumId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["Id"];
            };

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            var id = list.FirstOrDefault();
            if (id > 0)
            {
                var team = this.GetTeammates(teammate.TeamId);
                var retTeammate = team.FirstOrDefault(tm => tm.Id == id);

                return new RepositoryActionResult<Teammate>(retTeammate, RepositoryActionStatus.Created);
            }
            else
            {
                return new RepositoryActionResult<Teammate>(null, RepositoryActionStatus.Error);
            }
        }
    }
}