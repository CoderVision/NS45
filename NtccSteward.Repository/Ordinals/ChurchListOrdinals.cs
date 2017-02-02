using System.Data.SqlClient;

namespace NtccSteward.Repository.Ordinals
{
    public class ChurchListOrdinals
    {
        public ChurchListOrdinals(SqlDataReader reader)
        {
            Id = reader.GetOrdinal("ID");
            ChurchName = reader.GetOrdinal("Name");
            StatusId = reader.GetOrdinal("StatusId");
            Status = reader.GetOrdinal("Status");
            PastorId = reader.GetOrdinal("PastorId");
            Pastor = reader.GetOrdinal("Pastor");
            Phone = reader.GetOrdinal("Phone");
            Email = reader.GetOrdinal("Email");
            Address = reader.GetOrdinal("Address");
        }

        public int Id { get; set; }
        public int ChurchName { get; set; }
        public int StatusId { get; set; }
        public int Status { get; set; }
        public int PastorId { get; set; }
        public int Pastor { get; set; }
        public int Phone { get; set; }
        public int Email { get; set; }
        public int Address { get; set; }
    }
}
