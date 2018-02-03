using NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Models.Members;
using NtccSteward.Core.Models.Report;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository
{
    public interface IReportsRepository
    {
        object GetReportData(ReportTypes reportType, List<KeyValuePair<string, string>> paramsCollection);
        ReportMetadata GetMetadata(int churchId, int userId);
    }

    public class ReportsRepository : NtccSteward.Repository.Repository, IReportsRepository
    {
        private readonly SqlCmdExecutor _executor;
        private readonly ICommonRepository commonRepository;

        public ReportsRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
            commonRepository = new CommonRepository(connectionString);
        }

        public object GetReportData(ReportTypes reportType, List<KeyValuePair<string, string>> paramsCollection)
        {
            if (reportType == ReportTypes.ActiveGuestList)
                return GetActiveGuestList(paramsCollection);
            else
                return null;
        }

        public List<ActiveGuestListReportData> GetActiveGuestList(List<KeyValuePair<string, string>> paramsCollection)
        {
            var churchId = paramsCollection.FirstOrDefault(p => p.Key.Equals("churchId")).Value;
            //var period = paramsCollection.FirstOrDefault(p => p.Key.Equals("period")).Value;
            //var date = paramsCollection.FirstOrDefault(p => p.Key.Equals("date")).Value;
            var statusIds = paramsCollection.FirstOrDefault(p => p.Key.Equals("statusIds")).Value;
            var teamId = paramsCollection.FirstOrDefault(p => p.Key.Equals("teamId")).Value;
            var sponsorId = paramsCollection.FirstOrDefault(p => p.Key.Equals("sponsorId")).Value;

            var idlist = statusIds.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var arylist = Array.ConvertAll<string, int>(idlist, int.Parse);
            var statusTable = new DataTable();
            statusTable.Columns.Add("Id", typeof(int));
            arylist.ToList().ForEach(s => statusTable.Rows.Add(s));

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchId));
            paramz.Add(new SqlParameter("teamId", teamId));
            paramz.Add(new SqlParameter("sponsorId", sponsorId));
            paramz.Add(new SqlParameter("statusEnumIDs", statusTable));

            Func<SqlDataReader, ActiveGuestListReportData> readFx = (reader) =>
            {
                var activityDateIdx = reader.GetOrdinal("ActivityDate");
                var data = new ActiveGuestListReportData();
                data.LastActivityDate = reader.IsDBNull(activityDateIdx) ? (DateTimeOffset?)null : reader.ValueOrDefault<DateTimeOffset>(activityDateIdx);
                data.MemberId = reader.ValueOrDefault<int>("ID");
                data.MemberName = reader.ValueOrDefault<string>("Name");
                data.MemberAddress = reader.ValueOrDefault<string>("Address");
                data.MemberEmail = reader.ValueOrDefault<string>("Email");
                data.MemberPhone = reader.ValueOrDefault<string>("Number");
                data.SponsorId = reader.ValueOrDefault<int>("SponsorId");
                data.SponsorName = reader.ValueOrDefault<string>("Sponsor");
                data.TeamId = reader.ValueOrDefault<int>("TeamId");
                data.TeamName = reader.ValueOrDefault<string>("TeamName");
                data.Comments = reader.ValueOrDefault<string>("Comment");
                data.StatusId = reader.ValueOrDefault<int>("StatusId");
                data.Status = reader.ValueOrDefault<string>("Status");
                data.StatusChangeTypeId = reader.ValueOrDefault<int>("StatusChangeTypeId");
                data.StatusChangeType = reader.ValueOrDefault<string>("StatusChangeType");
                return data;
            };

            var list = _executor.ExecuteSql<ActiveGuestListReportData>("GetActiveGuestListReport", CommandType.StoredProcedure, paramz, readFx);

            return list;
        }

        public ReportMetadata GetMetadata(int churchId, int userId)
        {
            var metadata = new ReportMetadata();

            using (var cn = new SqlConnection(_executor.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetReportMetadata", cn))
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

                                metadata.Enums.Add(appEnum);
                            }

                            // read churches
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var church = new Church();
                                church.id = reader.ValueOrDefault<int>("Id");
                                church.Name = reader.ValueOrDefault<string>("Name");

                                metadata.Churches.Add(church);
                            }

                            // read teams
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var team = new NtccSteward.Core.Models.Team.Team();
                                team.Id = reader.ValueOrDefault<int>("TeamId");
                                team.Name = reader.ValueOrDefault<string>("Name");

                                metadata.Teams.Add(team);
                            }

                            // read members
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var member = new Core.Models.Members.Member();
                                member.id = reader.ValueOrDefault<int>("Id");
                                member.FirstName = reader.ValueOrDefault<string>("FirstName");
                                member.LastName = reader.ValueOrDefault<string>("LastName");
                                member.TeamId = reader.ValueOrDefault<int>("TeamId");

                                metadata.Members.Add(member);
                            }
                        }
                    }
                }
            }

            return metadata;
        }
    }
}

