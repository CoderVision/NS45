using System.Data.SqlClient;

namespace NtccSteward.Api.Repository.Ordinals
{
    public class ChurchListOrdinals
    {
        public ChurchListOrdinals(SqlDataReader reader)
        {
            Id = reader.GetOrdinal("ID");
            ChurchName = reader.GetOrdinal("Name");
        }

        public int Id { get; set; }
        public int ChurchName { get; set; }
    }
}
