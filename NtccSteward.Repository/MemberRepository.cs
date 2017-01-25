
using NtccSteward.Repository.Framework;
using NtccSteward.Core.Models.Members;
using NtccSteward.Api.Repository.Ordinals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Data.SqlTypes;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Models.Common.CustomAttributes;
using NtccSteward.Core.Models.Common.Enums;


namespace NtccSteward.Api.Repository
{
    public interface IMemberRepository
    {
        RepositoryActionResult<NewMember> Add(NewMember member);
        List<Member> GetList(int churchId, int statusEnumId);
        List<Member> GetProfileMetadata(int identityTypeEnumID);

        MemberProfile Get(int id);

        RepositoryActionResult<MemberProfile> SaveProfile(MemberProfile memberProfile);

        RepositoryActionResult<Member> Delete(int id);
    }

    public class MemberRepository : Repository, IMemberRepository
    {
        private readonly SqlCmdExecutor _executor;

        public MemberRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
        }

        /// <summary>
        /// Adds a new member
        /// </summary>
        /// <param name="member"></param>
        /// <param name="createdByUserId">The ID of the user that is creating this member</param>
        /// <param name="churchId">The churchId of the church that this new member belongs</param>
        /// <returns>New ID of the member, or -1 if no id was returned</returns>
        public RepositoryActionResult<NewMember> Add(NewMember member)
        {
            var proc = "CreateMember";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("ChurchId", member.ChurchId));
            paramz.Add(new SqlParameter("createdByUserId", member.CreatedByUserId));
            paramz.Add(new SqlParameter("firstName", member.FirstName.ToSqlString()));
            paramz.Add(new SqlParameter("lastName", member.LastName.ToSqlString()));
            paramz.Add(new SqlParameter("dateCame", member.DateCame.ToSqlDateTime()));
            paramz.Add(new SqlParameter("isGroup", member.IsGroup));
            paramz.Add(new SqlParameter("prayed", member.Prayed));
            paramz.Add(new SqlParameter("line1", member.Line1.ToSqlString()));
            paramz.Add(new SqlParameter("city", member.City.ToSqlString()));
            paramz.Add(new SqlParameter("state", member.State.ToSqlString()));
            paramz.Add(new SqlParameter("zip", member.Zip.ToSqlString()));
            paramz.Add(new SqlParameter("phone", member.Phone.ToSqlString()));
            paramz.Add(new SqlParameter("phone2", member.Phone2.ToSqlString()));
            paramz.Add(new SqlParameter("email", member.Email.ToSqlString()));
            paramz.Add(new SqlParameter("sponsorId", member.SponsorId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["MemberId"];
            };

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            var memberId = list.FirstOrDefault();

            if (memberId != 0)
            {
                return new RepositoryActionResult<NewMember>(member, RepositoryActionStatus.Created);
            }
            else
                return new RepositoryActionResult<NewMember>(member, RepositoryActionStatus.NotFound);

        }


        public List<Member> GetList(int churchId, int statusEnumId)
        {
            var proc = "Membership_SelectByChurch";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("ChurchId", churchId));
            paramz.Add(new SqlParameter("StatusEnumID", statusEnumId));

            MemberListOrdinals o = null;

            Func<SqlDataReader, Member> readFx = (reader) =>
            {
                if (o == null)
                    o = new MemberListOrdinals(reader);

                var member = new Member();
                member.id = reader.GetInt32(o.Id);
                member.FirstName = reader.ValueOrDefault<string>(o.FirstName, string.Empty);
                member.LastName = reader.ValueOrDefault<string>(o.LastName, string.Empty);
                member.Status = reader.ValueOrDefault<string>(o.Status, string.Empty);
                member.StatusChangeType = reader.ValueOrDefault<string>(o.StatusChangeType, string.Empty);
                member.Email = reader.ValueOrDefault<string>(o.Email, string.Empty);
                member.Phone = reader.ValueOrDefault<string>(o.Phone, string.Empty);
                member.ActivityDate = reader[o.ActivityDate].ToString();

                return member;
            };

            var list = _executor.ExecuteSql<Member>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }


        public MemberProfile GetProfileMetadata()
        {
            MemberProfile member = null;

            var proc = "GetMemberProfileMetadata";

            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var enm = new AppEnum();
                        enm.ID = reader.ValueOrDefault<int>("EnumID");
                        enm.Desc = reader.ValueOrDefault<string>("EnumDesc");
                        enm.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                        enm.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");

                        member.MetaDataList.Add(enm);
                    }
                }
            }

            return member;
        }

        public MemberProfile Get(int id)
        {
            MemberProfile member = null;

            var proc = "GetMemberProfile";

            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("memberId", id);

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        // member info
                        member = new MemberProfile();
                        member.MemberId = reader.ValueOrDefault<int>("IdentityID");
                        member.FirstName = reader.ValueOrDefault("FirstName", string.Empty);
                        member.MiddleName = reader.ValueOrDefault("MiddleName", string.Empty);
                        member.LastName = reader.ValueOrDefault("LastName", string.Empty);
                        member.PreferredName = reader.ValueOrDefault("PreferredName", string.Empty);
                        member.Gender = reader.ValueOrDefault("Gender", string.Empty);
                        member.Married = reader.ValueOrDefault<bool>("Married", false);
                        member.Veteran = reader.ValueOrDefault<bool>("Veteran", false);
                        member.BirthDate = reader.ValueOrDefault<DateTime?>("DateOfBirth", null)?.ToShortDateString();
                        member.DateSaved = reader.ValueOrDefault<DateTime?>("DateSaved", null)?.ToShortDateString();
                        member.DateBaptizedHolyGhost = reader.ValueOrDefault<DateTime?>("DateBaptizedHolyGhost", null)?.ToShortDateString();
                        member.DateBaptizedWater = reader.ValueOrDefault<DateTime?>("DateBaptizedWater", null)?.ToShortDateString();

                        // address info
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var addy = new Address();
                            addy.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                            addy.ContactInfoType = reader.ValueOrDefault<int>("ContactInfoTypeEnumID");
                            addy.ContactInfoLocationType = reader.ValueOrDefault<int>("ContactInfoLocationTypeEnumID");
                            addy.Preferred = reader.ValueOrDefault<bool>("Preferred");
                            addy.Verified = reader.ValueOrDefault<bool>("Verified");
                            addy.Line1 = reader.ValueOrDefault<string>("Line1");
                            addy.Line2 = reader.ValueOrDefault<string>("Line2");
                            addy.Line3 = reader.ValueOrDefault<string>("Line3");
                            addy.City = reader.ValueOrDefault<string>("City");
                            addy.State = reader.ValueOrDefault<string>("State");
                            addy.Zip = reader.ValueOrDefault<string>("Zip");

                            member.AddressList.Add(addy);
                        }

                        // phone info
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var addy = new Phone();
                            addy.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                            addy.ContactInfoType = reader.ValueOrDefault<int>("ContactInfoTypeEnumID");
                            addy.ContactInfoLocationType = reader.ValueOrDefault<int>("ContactInfoLocationTypeEnumID");
                            addy.Preferred = reader.ValueOrDefault<bool>("Preferred");
                            addy.Verified = reader.ValueOrDefault<bool>("Verified");
                            addy.PhoneNumber = reader.ValueOrDefault<string>("Number");
                            addy.PhoneType = reader.ValueOrDefault<int>("PhoneTypeEnumID", 0);  // cell phone, home phone, etc.

                            member.PhoneList.Add(addy);
                        }

                        // email info
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var addy = new Email();
                            addy.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                            addy.ContactInfoType = reader.ValueOrDefault<int>("ContactInfoTypeEnumID");
                            addy.ContactInfoLocationType = reader.ValueOrDefault<int>("ContactInfoLocationTypeEnumID");
                            addy.Preferred = reader.ValueOrDefault<bool>("Preferred");
                            addy.Verified = reader.ValueOrDefault<bool>("Verified");
                            addy.EmailAddress = reader.ValueOrDefault<string>("Email");

                            member.EmailList.Add(addy);
                        }

                        // attributes
                        //reader.NextResult();
                        //while (reader.Read())
                        //{
                        //    var attr = new CustomAttribute();
                        //    attr.IdentityID = reader.ValueOrDefault<int>("IdentityID");
                        //    attr.CustomAttrDefID = reader.ValueOrDefault<int>("CustomAttrDefID");
                        //    attr.Value = reader.ValueOrDefault<string>("Value", string.Empty);
                        //    attr.Name = reader.ValueOrDefault<string>("CustomAttrName", string.Empty);
                        //    attr.DataType = reader.ValueOrDefault<string>("DataType", string.Empty);
                        //    attr.AttrTypeEnumID = reader.ValueOrDefault<int>("AttrTypeEnumID");
                        //    attr.AttrTypeEnumDesc = reader.ValueOrDefault<string>("AttrTypeEnumDesc", string.Empty);
                        //    attr.IsEditable = reader.ValueOrDefault<bool>("IsEditable");

                        //    member.CustomAttributeList.Add(attr);
                        //}

                    }
                }
            }

            return member;
        }

        public List<Member> GetProfileMetadata(int identityTypeEnumID)
        {
            var list = new List<Member>();

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("identityTypeEnumID", identityTypeEnumID));

            Func<SqlDataReader, AppEnum> readFx = (reader) =>
            {
                var appEnum = new AppEnum();
                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");

                return appEnum;
            };

            _executor.ExecuteSql<AppEnum>("GetMemberProfile", CommandType.StoredProcedure, paramz, readFx);

            return list;
        }

        public RepositoryActionResult<MemberProfile> SaveProfile(MemberProfile memberProfile)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("memberId", memberProfile.MemberId));
            paramz.Add(new SqlParameter("firstName", memberProfile.FirstName.ToSqlString()));
            paramz.Add(new SqlParameter("middleName", memberProfile.MiddleName.ToSqlString()));
            paramz.Add(new SqlParameter("lastName", memberProfile.LastName.ToSqlString()));
            paramz.Add(new SqlParameter("preferredName", memberProfile.PreferredName.ToSqlString()));
            paramz.Add(new SqlParameter("birthDate", memberProfile.BirthDate.ToSqlString()));
            paramz.Add(new SqlParameter("gender", memberProfile.Gender.ToSqlString()));
            paramz.Add(new SqlParameter("comments", memberProfile.Comments.ToSqlString()));
            paramz.Add(new SqlParameter("dateSaved", memberProfile.DateSaved.ToSqlString()));
            paramz.Add(new SqlParameter("dateBaptizedWater", memberProfile.DateBaptizedWater.ToSqlString()));
            paramz.Add(new SqlParameter("dateBaptizedHolyGhost", memberProfile.DateBaptizedHolyGhost.ToSqlString()));
            paramz.Add(new SqlParameter("married", memberProfile.Married));
            paramz.Add(new SqlParameter("veteran", memberProfile.Veteran));
            paramz.Add(new SqlParameter("sponsorId", memberProfile.SponsorId));
            paramz.Add(new SqlParameter("memberStatusEnumType", memberProfile.MemberStatusEnumType));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["MemberId"];
            };

            var memberId = _executor.ExecuteSql<int>("SaveMemberProfile", CommandType.StoredProcedure, paramz, readFx);
            memberProfile.MemberId = memberId.FirstOrDefault();

            Func<SqlDataReader, int> ciReadFx = (reader) =>
            {
                return (int)reader["ContactInfoID"];
            };

            foreach (var addy in memberProfile.AddressList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("line1", addy.Line1.ToSqlString()));
                ciParamz.Add(new SqlParameter("line2", addy.Line2.ToSqlString()));
                ciParamz.Add(new SqlParameter("line3", addy.Line3.ToSqlString()));
                ciParamz.Add(new SqlParameter("city", addy.City.ToSqlString()));
                ciParamz.Add(new SqlParameter("state", addy.State.ToSqlString()));
                ciParamz.Add(new SqlParameter("zip", addy.Zip.ToSqlString()));

                var list = _executor.ExecuteSql<int>("SaveAddress", CommandType.StoredProcedure, ciParamz, ciReadFx);

                addy.ContactInfoId = list.First();
            }

            foreach (var addy in memberProfile.PhoneList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("@number", addy.PhoneNumber.ToSqlString()));
                ciParamz.Add(new SqlParameter("@phoneType", addy.PhoneType));

                var list = _executor.ExecuteSql<int>("SavePhone", CommandType.StoredProcedure, ciParamz, ciReadFx);

                addy.ContactInfoId = list.First();
            }

            foreach (var addy in memberProfile.EmailList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("@email", addy.EmailAddress.ToSqlString()));

                var list = _executor.ExecuteSql<int>("SaveEmail", CommandType.StoredProcedure, ciParamz, ciReadFx);

                addy.ContactInfoId = list.First();
            }

            if (memberProfile.MemberId != 0)
                return new RepositoryActionResult<MemberProfile>(memberProfile, RepositoryActionStatus.Updated);
            else
                return new RepositoryActionResult<MemberProfile>(memberProfile, RepositoryActionStatus.NotFound);
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


        public RepositoryActionResult<Member> Delete(int id)
        {
            try
            {
                // DO NOT DELETE, archive or tag as deleted

                //  add code to archive record in database.  return count.
                int deleteCount = 0;
                if (deleteCount == 1)
                {
                    return new RepositoryActionResult<Member>(null, RepositoryActionStatus.Deleted);
                }

                return new RepositoryActionResult<Member>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Member>(null, RepositoryActionStatus.Error, ex);
            }
        }


    }
}

