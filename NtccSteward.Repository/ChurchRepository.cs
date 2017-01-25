using NtccSteward.Api.Repository.Ordinals;
using NtccSteward.Core.Models.Church;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Api.Repository
{
    public interface IChurchRepository
    {
        void Add(Church church);
        Church GetById(int id);
        List<Church> GetList(bool showAll);
        bool TryDelete(int id);
    }

    public class ChurchRepository : Repository, IChurchRepository
    {
        public ChurchRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
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
    }
}
