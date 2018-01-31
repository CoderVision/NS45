
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
        List<AppEnum> GetProfileMetadata(int churchId, int userId);
        List<MemberSearchResult> SearchMembers(string criteria);

        MemberProfile GetProfile(int id, int churchId);

        RepositoryActionResult<MemberProfile> SaveProfile(MemberProfile memberProfile);

        RepositoryActionResult<Member> Delete(int id, int entityType);

        RepositoryActionResult<Activity> SaveActivity(Activity activity);
    }

    public class MemberRepository : NtccSteward.Repository.Repository, IMemberRepository
    {
        private readonly SqlCmdExecutor _executor;
        private readonly ICommonRepository commonRepository;

        public MemberRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);

            commonRepository = new CommonRepository(connectionString);
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
            paramz.Add(new SqlParameter("dateCame", member.DateCame));
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


        public List<MemberSearchResult> SearchMembers(string criteria)
        {
            var proc = "SearchMembersByName";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("criteria", criteria));

            MemberListOrdinals o = null;

            Func<SqlDataReader, MemberSearchResult> readFx = (reader) =>
            {
                var member = new MemberSearchResult();
                member.MemberId = (int)reader["MemberId"];
                member.FirstName = reader["FirstName"].ToString();
                member.LastName = reader["LastName"].ToString();
                member.ChurchId = (int)reader["ChurchId"];
                member.ChurchName = reader["ChurchName"].ToString();

                return member;
            };

            var list = _executor.ExecuteSql<MemberSearchResult>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }
        public List<Member> GetList(int churchId, IEnumerable<int> statusEnumIds)
        {
            var proc = "GetMembership";

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
                member.StatusId = reader.ValueOrDefault<int>("StatusId");
                member.Status = reader.ValueOrDefault<string>(o.Status, string.Empty);
                member.StatusChangeTypeId = reader.ValueOrDefault<int>("StatusChangeTypeId");
                member.StatusChangeType = reader.ValueOrDefault<string>(o.StatusChangeType, string.Empty);
                member.Email = reader.ValueOrDefault<string>(o.Email, string.Empty);
                member.Phone = reader.ValueOrDefault<string>(o.Phone, string.Empty);
                member.Address = reader.ValueOrDefault<string>(o.Address, string.Empty);
                member.ActivityDate = reader.ValueOrDefault<DateTimeOffset?>(o.ActivityDate, null);
                member.SponsorId = reader.ValueOrDefault<int>("SponsorId");
                member.Sponsor = reader.ValueOrDefault("Sponsor", string.Empty);
                member.TeamId = reader.ValueOrDefault<int>("TeamId");
                member.Team = reader.ValueOrDefault("TeamName", string.Empty);

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

        public List<AppEnum> GetProfileMetadata(int churchId, int userId)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchId));
            paramz.Add(new SqlParameter("userId", userId));

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
                commonRepository.MergeAddress(addy);
            }

            // Save Phone
            foreach (var addy in memberProfile.PhoneList)
            {
                commonRepository.MergePhone(addy);
            }

            // Save Email
            foreach (var addy in memberProfile.EmailList)
            {
                commonRepository.MergeEmail(addy);
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

        public RepositoryActionResult<Activity> SaveActivity(Activity activity)
        {
            try
            {
                var paramz = new List<SqlParameter>();
                paramz.Add(new SqlParameter("id", activity.Id));
                paramz.Add(new SqlParameter("churchId", activity.ChurchId));
                paramz.Add(new SqlParameter("sourceId", activity.SourceId));
                paramz.Add(new SqlParameter("targetId", activity.TargetId));
                paramz.Add(new SqlParameter("activityTypeEnumID", activity.ActivityTypeEnumID));
                paramz.Add(new SqlParameter("activityResponseTypeEnumID", activity.ActivityResponseTypeEnumID));
                paramz.Add(new SqlParameter("memberStatusChangeTypeEnumId", activity.MemberStatusChangeTypeEnumId));
                paramz.Add(new SqlParameter("memberStatusEnumId", activity.MemberStatusEnumId));
                paramz.Add(new SqlParameter("note", activity.Note));
                paramz.Add(new SqlParameter("createdDate", activity.CreatedDate));
                paramz.Add(new SqlParameter("activityDate", activity.ActivityDate));

                Func<SqlDataReader, int> readFx = (reader) =>
                {
                    return (int)reader["id"];
                };

                var list = _executor.ExecuteSql<int>("SaveActivity", CommandType.StoredProcedure, paramz, readFx);

                if (list != null && list.Any())
                {
                    activity.Id = list.First();

                    return new RepositoryActionResult<Activity>(activity, RepositoryActionStatus.Created);
                }
                else
                    return new RepositoryActionResult<Activity>(activity, RepositoryActionStatus.Error);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Activity>(activity, RepositoryActionStatus.Error, ex);
            }
        }
    }
}

