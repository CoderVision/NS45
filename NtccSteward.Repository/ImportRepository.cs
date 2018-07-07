using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository
{
    public interface IImportRepository
    {
        int SaveAccessDbFilePath(int churchid, string filePath);
        int SaveImportMember(int accessDbFilePathId, int memberId, int acessDbId, int importTableId);
    }

    public class ImportRepository : Repository, IImportRepository
    {
        private readonly SqlCmdExecutor _executor;
        private readonly ICommonRepository commonRepository;

        public ImportRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);

            commonRepository = new CommonRepository(connectionString);
        }

        public int SaveAccessDbFilePath(int churchid, string filePath)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchid));
            paramz.Add(new SqlParameter("accessDbFilePath", filePath));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["Id"];
            };

            var list = _executor.ExecuteSql<int>("AddImportFilePath", CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }

        public int SaveImportMember(int accessDbFilePathId, int memberId, int acessDbId, int importTableId)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("accessDbFilePathId", accessDbFilePathId));
            paramz.Add(new SqlParameter("memberId", memberId));
            paramz.Add(new SqlParameter("acessDbId", acessDbId));
            paramz.Add(new SqlParameter("importTableId", importTableId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["Id"];
            };

            var list = _executor.ExecuteSql<int>("AddImportMember", CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }
    }
}