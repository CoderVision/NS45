
using NtccSteward.Repository.Framework;
using NtccSteward.Core.Models.Members;
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
using NtccSteward.Repository.Ordinals;
using NtccSteward.Core.Models.Team;

namespace NtccSteward.Repository
{
    public interface IMemberRepository
    {
        RepositoryActionResult<NewMember> Add(NewMember member);
        List<Member> GetList(int churchId, IEnumerable<int> statusEnumId);
        List<AppEnum> GetProfileMetadata(int churchId);

        MemberProfile GetProfile(int id, int churchId);

        RepositoryActionResult<MemberProfile> SaveProfile(MemberProfile memberProfile);

        RepositoryActionResult<Member> Delete(int id, int entityType);

        RepositoryActionResult<Address> MergeAddress(Address addy);

        RepositoryActionResult<Phone> MergePhone(Phone phone);

        RepositoryActionResult<Email> MergeEmail(Email email);
    }

    public class MemberRepository : NtccSteward.Repository.Repository, IMemberRepository
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

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            member.SponsorList.ToList().ForEach(s => table.Rows.Add(s.SponsorId));
            paramz.Add(new SqlParameter("sponsorIds", table));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["MemberId"];
            };

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            var memberId = list.FirstOrDefault();

            if (memberId != 0)
            {
                member.id = memberId;

                return new RepositoryActionResult<NewMember>(member, RepositoryActionStatus.Created);
            }
            else
                return new RepositoryActionResult<NewMember>(member, RepositoryActionStatus.NotFound);

        }


        public List<Member> GetList(int churchId, IEnumerable<int> statusEnumIds)
        {
            var proc = "Membership_SelectByChurch";

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            statusEnumIds.ToList().ForEach(s => table.Rows.Add(s));

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("ChurchId", churchId));
            paramz.Add(new SqlParameter("StatusEnumIDs", table));

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
                member.Address = reader.ValueOrDefault<string>(o.Address, string.Empty);
                member.ActivityDate = reader.ValueOrDefault<DateTime?>(o.ActivityDate, null);

                return member;
            };

            var list = _executor.ExecuteSql<Member>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }



        public MemberProfile GetProfile(int id, int churchId)
        {
            MemberProfile member = null;

            var proc = "GetMemberProfile";

            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("memberId", id);
                cmd.Parameters.AddWithValue("churchId", churchId);

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
                        member.BirthDate = reader.ValueOrDefault<DateTime?>("DateOfBirth", null);
                        member.DateSaved = reader.ValueOrDefault<DateTime?>("DateSaved", null);
                        member.DateBaptizedHolyGhost = reader.ValueOrDefault<DateTime?>("DateBaptizedHolyGhost", null);
                        member.DateBaptizedWater = reader.ValueOrDefault<DateTime?>("DateBaptizedWater", null);
                        member.ChurchId = reader.ValueOrDefault<int>("ChurchId", 0);
                        member.ChurchName = reader["ChurchName"].ToString();
                        member.StatusId = reader.ValueOrDefault<int>("StatusId", 0);
                        member.StatusDesc = reader.ValueOrDefault("StatusDesc", string.Empty);
                        member.StatusChangeTypeId = reader.ValueOrDefault<int>("StatusChangeTypeId", 0);
                        member.StatusChangeTypeDesc = reader.ValueOrDefault("StatusChangeTypeDesc", string.Empty);
                        member.SponsorId = reader.ValueOrDefault<int>("SponsorId", 0);
                        member.Sponsor = reader.ValueOrDefault("Sponsor", string.Empty);
                        member.Comments = reader.ValueOrDefault("Comment", string.Empty);
                        member.MemberTypeEnumId = reader.ValueOrDefault<int>("MemberTypeEnumID", 0);

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

                        // Teams
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var team = new Core.Models.Members.Team();
                            team.MemberId = reader.ValueOrDefault<int>("MemberId");
                            team.TeamId = reader.ValueOrDefault<int>("TeamId");
                            team.TeamName = reader.ValueOrDefault<string>("TeamName");

                            member.TeamList.Add(team);
                        }

                        // Sponsors
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var sponsor = new Sponsor();
                            sponsor.MemberId = reader.ValueOrDefault<int>("MemberId");
                            sponsor.SponsorId = reader.ValueOrDefault<int>("SponsorId");
                            sponsor.FirstName = reader.ValueOrDefault<string>("FirstName");
                            sponsor.LastName = reader.ValueOrDefault<string>("LastName");

                            member.SponsorList.Add(sponsor);
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

            var list = _executor.ExecuteSql<AppEnum>("GetMemberProfileMetadata", CommandType.StoredProcedure, paramz, readFx);

            return list;
        }

        public RepositoryActionResult<MemberProfile> SaveProfile(MemberProfile memberProfile)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("memberId", memberProfile.MemberId));
            paramz.Add(new SqlParameter("churchId", memberProfile.ChurchId));
            paramz.Add(new SqlParameter("firstName", memberProfile.FirstName.ToSqlString()));
            paramz.Add(new SqlParameter("middleName", memberProfile.MiddleName.ToSqlString()));
            paramz.Add(new SqlParameter("lastName", memberProfile.LastName.ToSqlString()));
            paramz.Add(new SqlParameter("preferredName", memberProfile.PreferredName.ToSqlString()));
            paramz.Add(new SqlParameter("birthDate", memberProfile.BirthDate.ToSqlDateTime()));
            paramz.Add(new SqlParameter("gender", memberProfile.Gender.ToSqlString()));
            paramz.Add(new SqlParameter("comments", memberProfile.Comments.ToSqlString()));
            paramz.Add(new SqlParameter("dateSaved", memberProfile.DateSaved.ToSqlDateTime()));
            paramz.Add(new SqlParameter("dateBaptizedWater", memberProfile.DateBaptizedWater.ToSqlDateTime()));
            paramz.Add(new SqlParameter("dateBaptizedHolyGhost", memberProfile.DateBaptizedHolyGhost.ToSqlDateTime()));
            paramz.Add(new SqlParameter("married", memberProfile.Married));
            paramz.Add(new SqlParameter("veteran", memberProfile.Veteran));
            paramz.Add(new SqlParameter("sponsorId", memberProfile.SponsorId));
            paramz.Add(new SqlParameter("memberStatusEnumId", memberProfile.StatusId));
            paramz.Add(new SqlParameter("statusChangeTypeEnumId", memberProfile.StatusChangeTypeId));
            paramz.Add(new SqlParameter("memberTypeEnumId", memberProfile.MemberTypeEnumId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["MemberId"];
            };

            var memberId = _executor.ExecuteSql<int>("SaveMemberProfile", CommandType.StoredProcedure, paramz, readFx);
            memberProfile.MemberId = memberId.FirstOrDefault();

            // Save Address
            foreach (var addy in memberProfile.AddressList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("line1", addy.Line1.ToSqlString()));
                ciParamz.Add(new SqlParameter("line2", addy.Line2.ToSqlString()));
                ciParamz.Add(new SqlParameter("line3", addy.Line3.ToSqlString()));
                ciParamz.Add(new SqlParameter("city", addy.City.ToSqlString()));
                ciParamz.Add(new SqlParameter("state", addy.State.ToSqlString()));
                ciParamz.Add(new SqlParameter("zip", addy.Zip.ToSqlString()));

                var list = _executor.ExecuteSql<int>("SaveAddress", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

                addy.ContactInfoId = list.First();
            }

            // Save Phone
            foreach (var addy in memberProfile.PhoneList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("@number", addy.PhoneNumber.ToSqlString()));
                ciParamz.Add(new SqlParameter("@phoneType", addy.PhoneType));

                var list = _executor.ExecuteSql<int>("SavePhone", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

                addy.ContactInfoId = list.First();
            }

            // Save Email
            foreach (var addy in memberProfile.EmailList)
            {
                var ciParamz = CreateAddressInfoParams(addy);
                ciParamz.Add(new SqlParameter("@email", addy.EmailAddress.ToSqlString()));

                var list = _executor.ExecuteSql<int>("SaveEmail", CommandType.StoredProcedure, ciParamz, ContactInfoReadFx);

                addy.ContactInfoId = list.First();
            }

            // Save Sponsors
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            memberProfile.SponsorList.ToList().ForEach(s => table.Rows.Add(s.SponsorId));
            var sponsorParamz = new List<SqlParameter>();
            sponsorParamz.Add(new SqlParameter("memberId", memberProfile.MemberId));
            sponsorParamz.Add(new SqlParameter("sponsorIds", table));

            _executor.ExecuteSql<int>("SaveSponsor", CommandType.StoredProcedure, sponsorParamz, null);

            // Save Teams
            table.Rows.Clear();
            memberProfile.TeamList.ToList().ForEach(s => table.Rows.Add(s.TeamId));
            var teamParamz = new List<SqlParameter>();
            teamParamz.Add(new SqlParameter("memberId", memberProfile.MemberId));
            teamParamz.Add(new SqlParameter("teamIds", table));

            _executor.ExecuteSql<int>("SaveMemberTeams", CommandType.StoredProcedure, teamParamz, null);

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

        /// <summary>
        /// Deletes a person or contact info
        /// </summary>
        /// <param name="id">Person.IdentityId</param>
        /// <param name="entityType">EnumTypeId=12:  56 person, 61 ContactInfo</param>
        /// <returns></returns>
        public RepositoryActionResult<Member> Delete(int id, int entityType)
        {
            try
            {
                var paramz = new List<SqlParameter>();
                paramz.Add(new SqlParameter("id", id));
                paramz.Add(new SqlParameter("entityType", entityType));

                Func<SqlDataReader, int> readFx = (reader) =>
                {
                    return (int)reader["id"];
                };

                var list = _executor.ExecuteSql<int>("DeleteEntity", CommandType.StoredProcedure, paramz, readFx);

                if (list != null && list.Any())
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

        private int ContactInfoReadFx(SqlDataReader reader)
        {
            return (int)reader["ContactInfoID"];
        }
    }
}

