using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository
{
    public interface ICommonRepository
    {
        RepositoryActionResult<Address> MergeAddress(Address addy);

        RepositoryActionResult<Phone> MergePhone(Phone phone);

        RepositoryActionResult<Email> MergeEmail(Email email);
    }

    public class CommonRepository : NtccSteward.Repository.Repository, ICommonRepository
    {
        private readonly SqlCmdExecutor _executor;

        public CommonRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
        }

        public RepositoryActionResult<Email> MergeEmail(Email email)
        {
            var ciParamz = CreateAddressInfoParams(email);
            ciParamz.Add(new SqlParameter("@email", email.EmailAddress.ToSqlString()));

            var list = _executor.ExecuteSql<int>("SaveEmail", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

            var contactInfoId = list.First();

            if (email.ContactInfoId == contactInfoId)
                return new RepositoryActionResult<Email>(email, RepositoryActionStatus.Ok);
            else
            {
                email.ContactInfoId = contactInfoId;

                return new RepositoryActionResult<Email>(email, RepositoryActionStatus.Created);
            }
        }

        public RepositoryActionResult<Phone> MergePhone(Phone phone)
        {
            var ciParamz = CreateAddressInfoParams(phone);
            ciParamz.Add(new SqlParameter("@number", phone.PhoneNumber.ToSqlString()));
            ciParamz.Add(new SqlParameter("@phoneType", phone.PhoneType));

            var list = _executor.ExecuteSql<int>("SavePhone", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

            var contactInfoId = list.First();

            if (phone.ContactInfoId == contactInfoId)
                return new RepositoryActionResult<Phone>(phone, RepositoryActionStatus.Ok);
            else
            {
                phone.ContactInfoId = contactInfoId;

                return new RepositoryActionResult<Phone>(phone, RepositoryActionStatus.Created);
            }
        }

        public RepositoryActionResult<Address> MergeAddress(Address addy)
        {
            var ciParamz = CreateAddressInfoParams(addy);
            ciParamz.Add(new SqlParameter("line1", addy.Line1.ToSqlString()));
            ciParamz.Add(new SqlParameter("line2", addy.Line2.ToSqlString()));
            ciParamz.Add(new SqlParameter("line3", addy.Line3.ToSqlString()));
            ciParamz.Add(new SqlParameter("city", addy.City.ToSqlString()));
            ciParamz.Add(new SqlParameter("state", addy.State.ToSqlString()));
            ciParamz.Add(new SqlParameter("zip", addy.Zip.ToSqlString()));

            var list = _executor.ExecuteSql<int>("SaveAddress", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

            var contactInfoId = list.First();

            if (addy.ContactInfoId == contactInfoId)
                return new RepositoryActionResult<Address>(addy, RepositoryActionStatus.Ok);
            else
            {
                addy.ContactInfoId = contactInfoId;

                return new RepositoryActionResult<Address>(addy, RepositoryActionStatus.Created);
            }
        }

        private List<SqlParameter> CreateAddressInfoParams(IAddressInfo info)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("contactInfoId", info.ContactInfoId));
            paramz.Add(new SqlParameter("identityId", info.IdentityId));
            paramz.Add(new SqlParameter("modifiedByIdentityId", info.ModifiedByIdentityId));
            paramz.Add(new SqlParameter("contactInfoLocationType", info.ContactInfoLocationType));
            paramz.Add(new SqlParameter("preferred", info.Preferred));
            paramz.Add(new SqlParameter("verified", info.Verified));
            paramz.Add(new SqlParameter("archived", info.Archived));
            paramz.Add(new SqlParameter("note", info.Note.ToSqlString()));

            return paramz;
        }

        private int ContactInfoReadFx(SqlDataReader reader)
        {
            return (int)reader["ContactInfoID"];
        }
    }
}