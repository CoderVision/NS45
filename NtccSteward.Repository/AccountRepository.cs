
using IdentityServer3.Core;
using NtccSteward.Core.Framework;
using NtccSteward.Core.Models.Account;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace NtccSteward.Repository
{
    public interface IAccountRepository
    {
        int CreateAccountRequest(AccountRequest accountRequest);
        string GetAccountRequestStatus(int accountRequestId);
        User Login(string email, string password);
        bool ChangePassword(AccountPasswordChange accountRequest);
        User GetUserProfile(int userId);
        List<AccountRequest> GetAccountRequests();
        List<UserProfile> GetUsers(bool active);
        List<Role> GetRoles();
        string ProcessAccountRequest(AccountRequest accountRequest);
    }


    public class AccountRepository : NtccSteward.Repository.Repository, IAccountRepository
    {
        private readonly string pepper;

        //public AccountRepository(IAppConfigProvider appConfigProvider)
        public AccountRepository(string connectionString, string pepper)
        {
            ConnectionString = connectionString;
            this.pepper = pepper;
        }

        /// <summary>
        /// Creates an account request for a new user.  New request will be in pending status until approved or denied.
        /// </summary>
        /// <param name="accountRequest">AccountRequest</param>
        /// <returns>The RowNo of the new account request.</returns>
        public int CreateAccountRequest(AccountRequest accountRequest)
        {
            var salt = Guid.NewGuid().ToString();  // note:  create a new salt every time they change their password
            var passwordHash = CreatePasswordHash(accountRequest.Password, salt);

            var proc = "[Security].[CreateAccountRequest]";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("firstName", accountRequest.FirstName));
            paramz.Add(new SqlParameter("lastName", accountRequest.LastName));
            paramz.Add(new SqlParameter("email", accountRequest.Email));
            paramz.Add(new SqlParameter("salt", salt));
            paramz.Add(new SqlParameter("passwordHash", passwordHash));
            //paramz.Add(new SqlParameter("pastorName", accountRequest.PastorName));
            paramz.Add(new SqlParameter("churchId", accountRequest.ChurchId));
            paramz.Add(new SqlParameter("line1", accountRequest.Line1));
            paramz.Add(new SqlParameter("city", accountRequest.City));
            paramz.Add(new SqlParameter("state", accountRequest.State));
            paramz.Add(new SqlParameter("zip", accountRequest.Zip));
            paramz.Add(new SqlParameter("comments", accountRequest.Comments));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return (int)reader["AccountRequestID"];
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }


        /// <summary>
        /// Creates an account request for a new user.  New request will be in pending status until approved or denied.
        /// </summary>
        /// <param name="accountRequest">AccountRequest</param>
        /// <returns>The RowNo of the new account request.</returns>
        public string GetAccountRequestStatus(int accountRequestId)
        {
            var proc = "[Security].[GetAccountRequestStatus]";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("accountRequestId", accountRequestId));

            Func<SqlDataReader, string> readFx = (reader) =>
            {
                return reader["Status"].ToString();
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<string>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }


        /// <summary>
        /// Get all account requests for review and approval or denial
        /// </summary>
        public List<AccountRequest> GetAccountRequests()
        {
            var proc = "[Security].[GetAccountRequests]";

            Func<SqlDataReader, AccountRequest> readFx = (reader) =>
            {
                var acctReq = new AccountRequest {
                    RequestId = (int)reader["AccountRequestID"],
                    FirstName = reader["FirstName"] + "",
                    LastName = reader["LastName"] + "",
                    Line1 = reader["Line1"] + "",
                    City = reader["City"] + "",
                    State = reader["State"] + "",
                    Zip = reader["Zip"] + "",
                    Email = reader["Email"] + "",
                    Comments = reader["Comments"] + "",
                    ChurchId = (int)reader["ChurchId"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    RoleId = (int)Roles.User // default to user
                };
                return acctReq;
            };

            var executor = new SqlCmdExecutor(ConnectionString);

            var list = executor.ExecuteSql<AccountRequest>(proc, CommandType.StoredProcedure, null, readFx);

            return list;
        }


        public string ProcessAccountRequest(AccountRequest accountRequest)
        {
            var proc = "[Security].[ProcessAccountRequest]";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("accountRequestId", accountRequest.RequestId));
            paramz.Add(new SqlParameter("approved", accountRequest.IsApproved));
            paramz.Add(new SqlParameter("denied", !accountRequest.IsApproved));
            paramz.Add(new SqlParameter("processedByUserID", accountRequest.ReviewerUserId));
            paramz.Add(new SqlParameter("defaultUserRoleId", Roles.User));
            paramz.Add(new SqlParameter("memberTypeEnumId", MemberType.Member));
            paramz.Add(new SqlParameter("roleId", accountRequest.RoleId));

            paramz.Add(new SqlParameter("firstName", accountRequest.FirstName));
            paramz.Add(new SqlParameter("lastName", accountRequest.LastName));
            paramz.Add(new SqlParameter("line1", accountRequest.Line1));
            paramz.Add(new SqlParameter("city", accountRequest.City));
            paramz.Add(new SqlParameter("state", accountRequest.State));
            paramz.Add(new SqlParameter("zip", accountRequest.Zip));
            paramz.Add(new SqlParameter("email", accountRequest.Email));
            paramz.Add(new SqlParameter("churchId", accountRequest.ChurchId));
            paramz.Add(new SqlParameter("comments", accountRequest.Comments));

            // pass all info as parameters

            Func<SqlDataReader, string> readFx = (reader) =>
            {
                return reader["Status"].ToString();
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<string>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }

        /// <summary>
        /// Change a user's password
        /// </summary>
        /// <param name="accountRequest">AccountPasswordChange</param>
        /// <returns>true if the password was changed, falst if it was not</returns>
        public bool ChangePassword(AccountPasswordChange accountRequest)
        {
            // perform login to validate the old credentials
            var spice = GetUserLoginSpice(accountRequest.Email);

            if (spice == null) return false;

            var oldPasswordHash = CreatePasswordHash(accountRequest.OldPassword, spice.Salt);

            // save new password.
            var salt = Guid.NewGuid().ToString();  // note:  create a new salt every time they change their password
            var passwordHash = CreatePasswordHash(accountRequest.NewPassword, salt);

            var proc = "[Security].[ChangePassword]";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("email", accountRequest.Email));
            paramz.Add(new SqlParameter("oldPasswordHash", passwordHash));
            paramz.Add(new SqlParameter("newPasswordHash", passwordHash));
            paramz.Add(new SqlParameter("newSalt", salt));

            Func<SqlDataReader, bool> readFx = (reader) =>
            {
                return (bool)reader["Success"];
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<bool>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.FirstOrDefault();
        }


        private string CreatePasswordHash(string password, string salt)
        {
            var saltPepper = string.Concat(salt, this.pepper);
            var passwordHash = CryptoHashProvider.ComputeHash(password, saltPepper);
            return passwordHash;
        }


        /// <summary>
        /// Gets a user's PersonIdentityID & Salt
        /// </summary>
        /// <param name="email">Email is the Username</param>
        /// <returns>LoginSpice</returns>
        private LoginSpice GetUserLoginSpice(string email)
        {
            var proc = "[Security].[Login_GetUserSpice]";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("userName", email));

            Func<SqlDataReader, LoginSpice> readFx = (reader) =>
            {
                var spice = new LoginSpice();
                spice.PersonIdentityID = (int)reader["PersonIdentityID"];
                spice.Salt = reader["Salt"].ToString();
                return spice;
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<LoginSpice>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.FirstOrDefault();
        }


        /// <summary>
        /// Logs a user in and returns their information and a session id
        /// </summary>
        /// <param name="personIdentityID"></param>
        /// <param name="passwordHash"></param>
        /// <returns>Login</returns>
        public User Login(string email, string password)
        {
            User user = null;
            var spice = GetUserLoginSpice(email);

            if (spice == null)
                return null;

            var passwordHash = CreatePasswordHash(password, spice.Salt);

            var proc = "[Security].[Login]";

            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(proc, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("personIdentityID", spice.PersonIdentityID));
                    cmd.Parameters.Add(new SqlParameter("passwordHash", passwordHash));

                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        user = new User();

                        // user info (only 1 row)
                        reader.Read();

                        user.Subject = (int)reader["PersonId"] + "";
                        user.UserName = reader["UserName"].ToString();
                        user.IsActive = bool.Parse(reader["Active"].ToString());

                        user.UserClaims.Add(new UserClaim() { Id = "1", Subject = user.Subject, ClaimType = Constants.ClaimTypes.GivenName, ClaimValue = reader["FirstName"].ToString() });
                        user.UserClaims.Add(new UserClaim() { Id = "2", Subject = user.Subject, ClaimType = Constants.ClaimTypes.FamilyName, ClaimValue = reader["LastName"].ToString() });

                        reader.NextResult();

                        // roles & permissions
                        Role role = null;
                        while (reader.Read())
                        {
                            if (role == null)
                            {
                                role = new Role();
                                role.RoleId = (int)reader["RoleID"];
                                role.RoleDesc = reader["RoleDesc"].ToString();

                                user.UserClaims.Add(new UserClaim() { Id = role.RoleId.ToString(), Subject = user.Subject, ClaimType = Constants.ClaimTypes.Role, ClaimValue = role.RoleDesc });
                            }

                            //var permission = new Permission();
                            //permission.PermissionID = reader.ValueOrDefault<int>("PermissionID", 0);
                            //permission.PermissionDesc = reader.ValueOrDefault<string>("PermissionDesc", "");
                            //permission.Value = reader.ValueOrDefault<int>("PermissionValueID", 0);
                            //role.Permissions.Add(permission);
                        }
                    }
                }
            }

            return user;
        }


        public User GetUserProfile(int userId)
        {
            User user = null;

            var proc = "[Security].[GetUser]";

            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(proc, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("personIdentityID", userId));

                    cn.Open();
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        user = new User();

                        // user info (only 1 row)
                        reader.Read();

                        user.Subject = (int)reader["PersonId"] + "";
                        user.UserName = reader["UserName"].ToString();
                        user.IsActive = bool.Parse(reader["Active"].ToString());

                        user.UserClaims.Add(new UserClaim() { Id = "1", Subject = user.Subject, ClaimType = Constants.ClaimTypes.GivenName, ClaimValue = reader["FirstName"].ToString() });
                        user.UserClaims.Add(new UserClaim() { Id = "2", Subject = user.Subject, ClaimType = Constants.ClaimTypes.FamilyName, ClaimValue = reader["LastName"].ToString() });

                        reader.NextResult();

                        // roles & permissions
                        Role role = null;
                        while (reader.Read())
                        {
                            if (role == null)
                            {
                                role = new Role();
                                role.RoleId = (int)reader["RoleID"];
                                role.RoleDesc = reader["RoleDesc"].ToString();

                                user.UserClaims.Add(new UserClaim() { Id = role.RoleId.ToString(), Subject = user.Subject, ClaimType = Constants.ClaimTypes.Role, ClaimValue = role.RoleDesc });
                            }
                        }
                    }
                }
            }

            return user;
        }

        public List<Role> GetRoles()
        {
            var proc = "[Security].[GetRoles]";

            Func<SqlDataReader, Role> readFx = (reader) =>
            {
                return new Role
                {
                    RoleId = (int)reader["RoleID"],
                    RoleDesc = reader["RoleDesc"].ToString(),
                };
            };

            var executor = new SqlCmdExecutor(ConnectionString);
            var list = executor.ExecuteSql<Role>(proc, CommandType.StoredProcedure, null, readFx);

            return list;
        }

        public List<UserProfile> GetUsers(bool active)
        {
            var users = new List<UserProfile>();

            var proc = "[Security].[GetUsers]";

            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(proc, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("active", active));

                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return users;

                        var userProfile = new UserProfile();
                        while (reader.Read())
                        {
                            var userId = int.Parse(reader["PersonId"].ToString());

                            if (userProfile.UserId != userId)
                            {
                                userProfile = new UserProfile();
                                userProfile.UserId = userId;
                                userProfile.FirstName = reader["FirstName"].ToString();
                                userProfile.LastName = reader["LastName"].ToString();
                                userProfile.Email = reader["UserName"].ToString();
                                userProfile.RoleId = (int)reader["RoleID"];
                                userProfile.RoleDesc = reader["RoleDesc"].ToString();
                                userProfile.Active = bool.Parse(reader["Active"].ToString());

                                users.Add(userProfile);
                            }

                            var churchId = reader["ChurchId"];
                            if (churchId != DBNull.Value)
                            {
                                userProfile.ChurchIds.Add((int)churchId);
                            }
                        }
                    }
                }
            }

            return users;
        }
    }
}
