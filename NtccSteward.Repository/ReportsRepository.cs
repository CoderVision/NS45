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
        object GetReportData(ReportTypes reportType, int churchId);
        ReportMetadata GetMetadata(int churchId);
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

        public object GetReportData(ReportTypes reportType, int churchId)
        {
            if (reportType == ReportTypes.ActiveGuestList)
                return GetActiveGuestList(churchId);
            else
                return null;
        }

        public List<ActiveGuestListReportData> GetActiveGuestList(int churchId)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchId));

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

        public ReportMetadata GetMetadata(int churchId)
        {
            return new ReportMetadata();
        }
    }
}

