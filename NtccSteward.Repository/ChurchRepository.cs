
using NtccSteward.Core.Interfaces.Common.Address;
using NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Common.Address;
using NtccSteward.Core.Models.Common.Enums;
using NtccSteward.Core.Models.Team;
using NtccSteward.Repository.Framework;
using NtccSteward.Repository.Ordinals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace NtccSteward.Repository
{
    public interface IChurchRepository
    {
        RepositoryActionResult<Church> Add(Church church);
        ChurchProfile Get(int id);
        List<Church> GetList(bool showAll);
        RepositoryActionResult<ChurchProfile> SaveProfile(ChurchProfile profile);
        RepositoryActionResult<Church> Delete(int id, int entityType);
        ChurchProfileMetadata GetProfileMetadata(int churchId);
    }

    public class ChurchRepository : NtccSteward.Repository.Repository, IChurchRepository
    {
        private readonly SqlCmdExecutor _executor;
        private readonly ICommonRepository commonRepository;

        public ChurchRepository(string connectionString)
        {
            this.ConnectionString = connectionString;

            _executor = new SqlCmdExecutor(connectionString);
            commonRepository = new CommonRepository(connectionString);
        }


        public RepositoryActionResult<Church> Add(Church church)
        {
            var proc = "CreateChurch";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("createdByUserId", church.CreatedByUserId));
            paramz.Add(new SqlParameter("name", church.Name.ToSqlString()));
            paramz.Add(new SqlParameter("pastorId", church.PastorId));
            paramz.Add(new SqlParameter("line1", church.Line1.ToSqlString()));
            paramz.Add(new SqlParameter("city", church.City.ToSqlString()));
            paramz.Add(new SqlParameter("state", church.State.ToSqlString()));
            paramz.Add(new SqlParameter("zip", church.Zip.ToSqlString()));
            paramz.Add(new SqlParameter("phone", church.Phone.ToSqlString()));
            paramz.Add(new SqlParameter("email", church.Email.ToSqlString()));
            paramz.Add(new SqlParameter("timeZoneOffset", church.TimeZoneOffset.ToSqlString()));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["ChurchId"];
            };

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            var churchId = list.FirstOrDefault();

            if (churchId != 0)
            {
                church.id = churchId;

                return new RepositoryActionResult<Church>(church, RepositoryActionStatus.Created);
            }
            else
                return new RepositoryActionResult<Church>(church, RepositoryActionStatus.NotFound);

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
                                church.StatusId = reader.ValueOrDefault<int>(o.StatusId, 0);
                                church.StatusDesc = reader.ValueOrDefault<string>(o.Status, string.Empty);
                                church.PastorId = reader.ValueOrDefault<int>(o.PastorId, 0);
                                church.Pastor = reader.ValueOrDefault<string>(o.Pastor, string.Empty);
                                church.Phone = reader.ValueOrDefault<string>(o.Phone, string.Empty);
                                church.Email = reader.ValueOrDefault<string>(o.Email, string.Empty);
                                church.Address = reader.ValueOrDefault<string>(o.Address, string.Empty);
                                list.Add(church);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public ChurchProfile Get(int id)
        {
            ChurchProfile church = null;

            var proc = "GetChurchProfile";

            using (var cn = new SqlConnection(_executor.ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("churchId", id);

                cn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        // member info
                        church = new ChurchProfile();
                        church.Id = reader.ValueOrDefault<int>("ChurchId");
                        church.Name = reader.ValueOrDefault("ChurchName", string.Empty);
                        church.StatusId = reader.ValueOrDefault<int>("StatusId", 0);
                        church.StatusDesc = reader.ValueOrDefault("StatusDesc", string.Empty);
                        church.Comment = reader.ValueOrDefault("Comment", string.Empty);
                        church.TimeZoneOffset = reader.ValueOrDefault("TimeZoneOffset", string.Empty);
                        church.SmsAccountSID = reader.ValueOrDefault("AccountSid", string.Empty);
                        church.SmsAccountToken = reader.ValueOrDefault("AccountToken", string.Empty);
                        church.EmailConfigProfileId = reader.ValueOrDefault<int>("EmailConfigProfileId");
                        church.EmailConfigUsername = reader.ValueOrDefault<string>("EmailConfigUsername");
                        church.EmailConfigPassword = reader.ValueOrDefault<string>("EmailConfigPassword");

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

                            church.AddressList.Add(addy);
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

                            church.PhoneList.Add(addy);
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

                            church.EmailList.Add(addy);
                        }

                        // Pastoral Team members
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var teammate = new Teammate();
                            teammate.Id = reader.ValueOrDefault<int>("TeammateId");
                            teammate.TeamId = reader.ValueOrDefault<int>("TeamId");
                            teammate.MemberId = reader.ValueOrDefault<int>("MemberId");
                            teammate.Name = reader.ValueOrDefault<string>("MemberName");
                            teammate.TeamPositionEnumId = reader.ValueOrDefault<int>("TeamPositionEnumId");
                            teammate.TeamPositionEnumDesc = reader.ValueOrDefault<string>("Position");

                            church.PastoralTeamMembers.Add(teammate);
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

            return church;
        }


        public ChurchProfileMetadata GetProfileMetadata(int churchId)
        {
            var metadata = new ChurchProfileMetadata();

            //var paramz = new List<SqlParameter>();
            //paramz.Add(new SqlParameter("churchId", churchId));

            //Func<SqlDataReader, AppEnum> readFx = (reader) =>
            //{
            //    var appEnum = new AppEnum();
            //    appEnum.ID = reader.ValueOrDefault<int>("EnumID");
            //    appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
            //    appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
            //    appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");

            //    return appEnum;
            //};

            //var list = _executor.ExecuteSql<AppEnum>("GetChurchProfileMetadata", CommandType.StoredProcedure, paramz, readFx);

            using (var cn = new SqlConnection(_executor.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetChurchProfileMetadata", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("churchId", churchId));
                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var appEnum = new AppEnum();
                                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");

                                metadata.Enums.Add(appEnum);
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                var emailProvider = new EmailProvider();
                                emailProvider.Id = reader.ValueOrDefault<int>("Id");
                                emailProvider.Name = reader.ValueOrDefault<string>("Name");
                                emailProvider.Server = reader.ValueOrDefault<string>("Server");
                                emailProvider.Port = reader.ValueOrDefault<int>("Port");

                                metadata.EmailProviders.Add(emailProvider);
                            }
                        }
                    }
                }
            }

            return metadata;
        }


        public RepositoryActionResult<ChurchProfile> SaveProfile(ChurchProfile profile)
        {
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", profile.Id));
            paramz.Add(new SqlParameter("name", profile.Name.ToSqlString()));
            paramz.Add(new SqlParameter("statusEnumId", profile.StatusId));
            paramz.Add(new SqlParameter("comments", profile.Comment.ToSqlString()));
            paramz.Add(new SqlParameter("timeZoneOffset", profile.TimeZoneOffset.ToSqlString()));
            paramz.Add(new SqlParameter("smsAccoundSid", profile.SmsAccountSID.ToSqlString()));
            paramz.Add(new SqlParameter("smsAccountToken", profile.SmsAccountToken.ToSqlString()));
            paramz.Add(new SqlParameter("emailConfigProfileId", profile.EmailConfigProfileId));
            paramz.Add(new SqlParameter("emailConfigUserName", profile.EmailConfigUsername.ToSqlString()));
            paramz.Add(new SqlParameter("emailConfigPassword", profile.EmailConfigPassword.ToSqlString()));

            Func <SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["ChurchId"];
            };

            var church = _executor.ExecuteSql<int>("SaveChurchProfile", CommandType.StoredProcedure, paramz, readFx);

            profile.Id = church.FirstOrDefault();

            Func<SqlDataReader, int> ciReadFx = (reader) =>
            {
                return (int)reader["ContactInfoID"];
            };

            foreach (var addy in profile.AddressList)
            {
                commonRepository.MergeAddress(addy);
            }

            foreach (var addy in profile.PhoneList)
            {
                commonRepository.MergePhone(addy);
            }

            foreach (var addy in profile.EmailList)
            {
                commonRepository.MergeEmail(addy);
            }

            if (profile.PastoralTeam != null)
            {
                // save team
                paramz.Clear();
                paramz.Add(new SqlParameter("id", profile.PastoralTeam.Id));
                paramz.Add(new SqlParameter("name", profile.PastoralTeam.Name.ToSqlString()));
                paramz.Add(new SqlParameter("churchId", profile.Id));
                paramz.Add(new SqlParameter("teamTypeEnumId", profile.PastoralTeam.TeamTypeEnumId));
                paramz.Add(new SqlParameter("teamPositionEnumTypeId", profile.PastoralTeam.TeamPositionEnumTypeId));

                readFx = (reader) =>
                {
                    return (int)reader["Id"];
                };

                var teamIds = _executor.ExecuteSql<int>("SaveTeam", CommandType.StoredProcedure, paramz, readFx);

                profile.PastoralTeam.Id = teamIds.FirstOrDefault();

                // Save Teammates
                foreach (var teamMate in profile.PastoralTeam.Teammates)
                {
                    teamMate.TeamId = profile.PastoralTeam.Id;

                    paramz.Clear();
                    paramz.Add(new SqlParameter("@teammateId", teamMate.Id));
                    paramz.Add(new SqlParameter("@teamId", teamMate.TeamId));
                    paramz.Add(new SqlParameter("@personId", teamMate.MemberId));
                    paramz.Add(new SqlParameter("@teamPositionEnumId", teamMate.TeamPositionEnumId));

                    var teammateIds = _executor.ExecuteSql<int>("SaveTeammate", CommandType.StoredProcedure, paramz, readFx);

                    teamMate.Id = teammateIds.FirstOrDefault();
                }
            }

            if (profile.Id != 0)
                return new RepositoryActionResult<ChurchProfile>(profile, RepositoryActionStatus.Updated);
            else
                return new RepositoryActionResult<ChurchProfile>(null, RepositoryActionStatus.NotFound);
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
        /// Deletes a church or contact info
        /// </summary>
        /// <param name="id">Church.IdentityId</param>
        /// <param name="entityType">EnumTypeId=12:  55 church, 61 ContactInfo</param>
        /// <returns></returns>
        public RepositoryActionResult<Church> Delete(int id, int entityType)
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
                    return new RepositoryActionResult<Church>(null, RepositoryActionStatus.Deleted);
                }

                return new RepositoryActionResult<Church>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Church>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}
