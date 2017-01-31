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
            Phone = reader.GetOrdinal("Number");
            Email = reader.GetOrdinal("Email");
            Address = reader.GetOrdinal("Address");
            ActivityDate = reader.GetOrdinal("ActivityDate");
        }

        public int Id { get; set; }
        public int FirstName { get; set; }
        public int LastName { get; set; }
        public int Status { get; set; }
        public int StatusChangeType { get; set; }
        public int Phone { get; set; }
        public int Email { get; set; }
        public int Address { get; set; }
        public int ActivityDate { get; set; }
    }
}