using NtccSteward.Api.Repository.Ordinals;
using NtccSteward.Core.Models.Church;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Models.Common.Enums;
using System.Data;

namespace NtccSteward.Api.Repository
{
    public interface IChurchRepository
    {
        void Add(Church church);
        Church GetById(int id);
        List<Church> GetList(bool showAll);
        bool TryDelete(int id);
        List<AppEnum> GetProfileMetadata(int churchId);
    }

    public class ChurchRepository : Repository, IChurchRepository
    {
        private readonly SqlCmdExecutor _executor;

        public ChurchRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
        }

        public void Add(Church church)
        {
            throw new NotImplementedException();
        }

        public List<Church> GetList(bool showAll)
        {
            var list = new List<Church>();

            using (var cn = new SqlConnection(this.ConnectionString))
            {
                using (var cmd = new SqlCommand("Church_Select", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("showAll", showAll);

                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var o = new ChurchListOrdinals(reader);

                            while (reader.Read())
                            {
                                var church = new Church();
                                church.id = reader.GetInt32(o.Id);
                                church.Name = reader.ValueOrDefault(o.ChurchName, string.Empty);
                                list.Add(church);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public Church GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool TryDelete(int id)
        {
            throw new NotImplementedException();

            //return true;
        }

        public List<AppEnum> GetProfileMetadata(int churchId)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchId));

            Func<SqlDataReader, AppEnum> readFx = (reader) =>
            {
                var appEnum = new AppEnum();
                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");

                return appEnum;
            };

            var list = _executor.ExecuteSql<AppEnum>("GetChurchProfileMetadata", CommandType.StoredProcedure, paramz, readFx);

            return list;
        }
    }
}
