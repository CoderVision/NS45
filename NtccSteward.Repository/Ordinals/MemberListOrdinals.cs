using System.Data.SqlClient;

namespace NtccSteward.Api.Repository.Ordinals
{
    public class MemberListOrdinals
    {
        public MemberListOrdinals(SqlDataReader reader)
        {
            Id = reader.GetOrdinal("ID");
            FirstName = reader.GetOrdinal("FirstName");
            LastName = reader.GetOrdinal("LastName");
            Status = reader.GetOrdinal("Status");
            StatusChangeType = reader.GetOrdinal("StatusChangeType");
        }

        public int Id { get; set; }
        public int FirstName { get; set; }
        public int LastName { get; set; }
        public int Status { get; set; }
        public int StatusChangeType { get; set; }
    }
}