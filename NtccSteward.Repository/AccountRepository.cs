
using NtccSteward.Core.Framework;
using NtccSteward.Core.Models.Account;
using NtccSteward.Repository.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace NtccSteward.Api.Repository
{
    public interface IAccountRepository
    {
        int CreateAccountRequest(AccountRequest accountRequest);
        Session Login(string email, string password, int churchId);
        bool ChangePassword(AccountPasswordChange accountRequest);
    }


    public class AccountRepository : Repository, IAccountRepository
    {
        private readonly string _pepper;

        //public AccountRepository(IAppConfigProvider appConfigProvider)
        public AccountRepository(string connectionString, string pepper)
        {
            ConnectionString = connectionString;
            _pepper = pepper;
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
            paramz.Add(new SqlParameter("pastorName", accountRequest.PastorName));
            paramz.Add(new SqlParameter("churchName", accountRequest.ChurchName));
            paramz.Add(new SqlParameter("city", accountRequest.City));
            paramz.Add(new SqlParameter("state", accountRequest.State));
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
            var saltPepper = string.Concat(salt, _pepper);
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
        public Session Login(string email, string password, int churchId)
        {
            var spice = GetUserLoginSpice(email);

            if (spice == null)
                return null;

            var passwordHash = CreatePasswordHash(password, spice.Salt);

            var session = new Session();

            var proc = "[Security].[Login]";

            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(proc, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("personIdentityID", spice.PersonIdentityID));
                    cmd.Parameters.Add(new SqlParameter("passwordHash", passwordHash));
                    cmd.Parameters.Add(new SqlParameter("churchId", churchId));

                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        // user info (only 1 row)
                        reader.Read();

                        session.UserId = (int)reader["PersonId"];
                        session.SessionId = reader["SessionID"].ToString();
                        session.ChurchId = (int)reader["ChurchId"];

                        reader.NextResult();

                        // roles & permissions
                        while (reader.Read())
                        {
                            var roleId = (int)reader["RoleID"];

                            var role = session.Roles.FirstOrDefault(r => r.RoleID == roleId);
                            if (role == null)
                            {
                                role = new Role();
                                role.RoleID = (int)reader["RoleID"];
                                role.RoleDesc = reader["RoleDesc"].ToString();
                                session.Roles.Add(role);
                            }

                            var permission = new Permission();
                            permission.PermissionID = reader.ValueOrDefault<int>("PermissionID", 0);
                            permission.PermissionDesc = reader.ValueOrDefault<string>("PermissionDesc", "");
                            permission.Value = reader.ValueOrDefault<int>("PermissionValueID", 0);
                            role.Permissions.Add(permission);
                        }
                    }
                }
            }

            return session;
        }
    }
}
