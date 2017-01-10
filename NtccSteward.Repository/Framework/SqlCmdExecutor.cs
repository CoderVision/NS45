using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Repository.Framework
{
    public interface ISqlCmdExecutor
    {
        List<T> ExecuteSql<T>(string sql, CommandType commandType, IEnumerable<SqlParameter> paramz, Func<SqlDataReader, T> readFx);
    }


    public class SqlCmdExecutor : ISqlCmdExecutor
    {
        public SqlCmdExecutor(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }



        /// <summary>
        /// Executes the sql and returns a list
        /// </summary>
        /// <typeparam name=" T">The type of object to return </typeparam>
        /// <param name=" sql"> The SQL to execute: proc or text</param>
        /// <param name=" commandType">CommandType </param>
        /// <param name=" paramz"> [optional] list of parameters</param>
        /// <param name=" readFx"> [optional] function that reads the data from an SqlDataReader</param>
        /// <returns></returns>
        public List<T> ExecuteSql<T>(string sql, CommandType commandType, IEnumerable<SqlParameter> paramz, Func<SqlDataReader, T> readFx)
        {
            var list = new List<T>();

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.CommandType = commandType;

                if (paramz != null)
                    cmd.Parameters.AddRange(paramz.ToArray());

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (readFx != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            T item = readFx(reader);

                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }
    }
}
