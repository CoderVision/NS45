
using NtccSteward.Core.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Api.Repository
{
    public interface ILogger
    {
        int LogInfo(LogLevel logLevel, string errSummary, string errDetails, int userId);
    }


    public class LoggerRepository : Repository, ILogger
    {
        public LoggerRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// logs the error and returns the log id.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="errSummary"></param>
        /// <param name="errDetails"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int LogInfo(LogLevel logLevel, string errSummary, string errDetails, int userId)
        {
            var proc = "LogError";
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@LogLevel", logLevel);
                cmd.Parameters.AddWithValue("@Summary", errSummary);
                cmd.Parameters.AddWithValue("@Details", errDetails);
                cn.Open();
                var logId = (int)cmd.ExecuteScalar();
                return logId;
            }
        }
    }
}
