using NtccSteward.Repository.Framework;
using NtccSteward.Core.Interfaces.Messages;
using NtccSteward.Core.Models.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Common.Enums;

namespace NtccSteward.Repository
{
    public interface IMessageRepository
    {
        void DeleteMessage(int messageID);
        void DeleteRecipient(int recipientId);
        void DeleteRecipientGroup(int recipientGroupId);
        List<IMessage> GetMessages(int recipietGroupId, int maxReturnRows);
        List<IRecipientGroup> GetRecipientGroups(int churchId, int messageTypeEnumId);
        int SaveMessage(IMessage message); 
        int SaveRecipient(IRecipient recipient);
        int SaveRecipientGroup(IRecipientGroup group);
        MessageMetadata GetMetadata(int userId);
        IRecipient GetRecipient(int contactInfoid, int recipientGroupId);
        List<IRecipient> GetRecipients(int churchId, int messageTypeEnumId, string criteria);
        SmsConfiguration GetSmsConfiguration(int recipientGroupId);
        List<IRecipient> GetGroupRecipients(int recipientGroupId);
        List<EmailConfigurationProfile> GetEmailConfigurationProfiles();
        EmailConfigurationProfile SaveEmailConfigurationProfiles(EmailConfigurationProfile emailConfiguration);
        void SaveEmailConfiguration(EmailConfiguration emailConfiguration);
    }

    public class MessageRepository : NtccSteward.Repository.Repository, IMessageRepository
    {
        private readonly ISqlCmdExecutor executor;

        public MessageRepository(string connectionString)
        {
            ConnectionString = connectionString;
            this.executor = new SqlCmdExecutor(connectionString);
        }

        public MessageMetadata GetMetadata(int userId)
        {
            var metadata = new MessageMetadata();

            using (var cn = new SqlConnection(this.executor.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetMessagesMetadata", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("userId", userId));
                    cn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // read enum types
                            while (reader.Read())
                            {
                                var appEnum = new AppEnum();
                                appEnum.ID = reader.ValueOrDefault<int>("EnumID");
                                appEnum.Desc = reader.ValueOrDefault<string>("EnumDesc");
                                appEnum.AppEnumTypeID = reader.ValueOrDefault<int>("EnumTypeID");
                                appEnum.AppEnumTypeName = reader.ValueOrDefault<string>("EnumTypeName");
                                metadata.Enums.Add(appEnum);
                            }

                            // read churches
                            reader.NextResult();
                            while (reader.Read())
                            {
                                var church = new Church();
                                church.id = reader.ValueOrDefault<int>("ChurchId");
                                church.Name = reader.ValueOrDefault<string>("Name");
                                metadata.Churches.Add(church);
                            }
                        }
                    }
                }
            }

            return metadata;
        }

        public List<IRecipient> GetGroupRecipients(int recipientGroupId)
        {
            var proc = "GetGroupRecipients";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("recipientGroupId", recipientGroupId));

            Func<SqlDataReader, IRecipient> readFx = (reader) =>
            {
                var item = new Recipient();
                item.Id = reader.ValueOrDefault<int>("Id");
                item.IdentityId = reader.ValueOrDefault<int>("IdentityId");
                item.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                item.Name = $"{reader.ValueOrDefault<string>("FirstName", "").Trim()} {reader.ValueOrDefault<string>("MiddleName", "").Trim()}".Trim() + $" {reader.ValueOrDefault<string>("LastName", "").Trim()}";
                item.Address = reader.ValueOrDefault<string>("Address");
                item.PreferredAddress = reader.ValueOrDefault<bool>("Preferred");
                item.MessageRecipientGroupId = reader.ValueOrDefault<int>("MessageRecipientGroupId");
                return item;
            };

            var list = this.executor.ExecuteSql<IRecipient>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }

        public List<IRecipient> GetRecipients(int churchId, int messageTypeEnumId, string criteria) {

            var proc = "GetRecipients";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchId));
            paramz.Add(new SqlParameter("criteria", criteria));
            paramz.Add(new SqlParameter("messageTypeEnumId", messageTypeEnumId));

            Func<SqlDataReader, IRecipient> readFx = (reader) =>
            {
                var item = new Recipient();
                item.IdentityId = reader.ValueOrDefault<int>("IdentityId");
                item.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                item.Name = $"{reader.ValueOrDefault<string>("FirstName", "").Trim()} {reader.ValueOrDefault<string>("MiddleName", "").Trim()}".Trim() + $" {reader.ValueOrDefault<string>("LastName", "").Trim()}";
                item.Address = reader.ValueOrDefault<string>("Address");
                item.PreferredAddress = reader.ValueOrDefault<bool>("Preferred");
                return item;
            };

            var list = this.executor.ExecuteSql<IRecipient>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }


        /// <summary>
        /// Gets message groups, does not include recipients
        /// </summary>
        /// <param name="churchID">churchID</param>
        /// <returns></returns>
        public List<IRecipientGroup> GetRecipientGroups(int churchID, int messageTypeEnumId)
        {
            var proc = "GetMessageRecipientGroups";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", churchID));
            paramz.Add(new SqlParameter("messageTypeEnumId", messageTypeEnumId));

            Func<SqlDataReader, IRecipientGroup> readFx = (reader) =>
            {
                var item = new RecipientGroup();
                item.Id = reader.ValueOrDefault<int>("Id");
                item.ChurchId = reader.ValueOrDefault<int>("ChurchID");
                item.Name = reader.ValueOrDefault<string>("Name");
                item.Description = reader.ValueOrDefault<string>("Description");
                item.LastMessageDate = reader.ValueOrDefault<DateTimeOffset>("LastMessageDate");
                item.LastMessageBody = reader.ValueOrDefault<string>("LastMessageBody");
                item.LastMessageSubject = reader.ValueOrDefault<string>("LastMessageSubject");
                item.MessageTypeEnumID = reader.ValueOrDefault<int>("MessageTypeEnumID");

                return item;
            };

            var list = this.executor.ExecuteSql<IRecipientGroup>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }


        /// <summary>
        /// Saves the group, returns the ID.
        /// </summary>
        /// <returns>The ID of the group (new if insert, same if update)</returns>
        public int SaveRecipientGroup(IRecipientGroup group)
        {
            var proc = "SaveMessageRecipientGroup";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("id", group.Id));
            paramz.Add(new SqlParameter("ChurchId", group.ChurchId));
            paramz.Add(new SqlParameter("Name", group.Name));
            paramz.Add(new SqlParameter("Desc", group.Description));
            paramz.Add(new SqlParameter("MessageTypeEnumId", group.MessageTypeEnumID));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return reader.GetInt32(0); // new id
            };

            var list = this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            // we need to do a merge of recipients

            return list.First();
        }


        public void DeleteRecipientGroup(int recipientGroupId)
        {
            var proc = "DeleteMessageRecipientGroup";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("id", recipientGroupId));

            this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
        }


        /// <summary>
        /// Saves the recipient, returns the ID.
        /// </summary>
        /// <returns>The ID of the recipient (new if insert, same if update)</returns>
        public int SaveRecipient(IRecipient recipient)
        {
            var proc = "SaveMessageRecipient";
            
            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("Id", recipient.Id));
            paramz.Add(new SqlParameter("IdentityId", recipient.IdentityId));
            paramz.Add(new SqlParameter("recipientGroupId", recipient.MessageRecipientGroupId));
            paramz.Add(new SqlParameter("ContactInfoId", recipient.ContactInfoId));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return reader.GetInt32(0); // new id
            };

            var list = this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }

        public IRecipient GetRecipient(int contactInfoid, int recipientGroupId)
        {
            var proc = "GetRecipient";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("contactInfoid", contactInfoid));
            paramz.Add(new SqlParameter("recipientGroupId", recipientGroupId));

            Func<SqlDataReader, IRecipient> readFx = (reader) =>
            {
                var item = new Recipient();
                item.Id = reader.ValueOrDefault<int>("Id");
                item.IdentityId = reader.ValueOrDefault<int>("IdentityID");
                item.MessageRecipientGroupId = reader.ValueOrDefault<int>("MessageRecipientGroupId");
                item.ContactInfoId = reader.ValueOrDefault<int>("ContactInfoID");
                item.Name = reader.ValueOrDefault<string>("Name");
                item.Address = reader.ValueOrDefault<string>("Address");

                return item;
            };

            var list = this.executor.ExecuteSql<IRecipient>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a message recipient, removing it from the group
        /// </summary>
        public void DeleteRecipient(int recipientId)
        {
            var proc = "DeleteMessageRecipient";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("Id", recipientId));

            this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
        }


        /// <summary>
        /// Saves a message by inserting a new record in the database.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The ID of the new message</returns>
        public int SaveMessage(IMessage message)
        {
            var proc = "SaveMessage";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("id", message.Id));
            paramz.Add(new SqlParameter("messageRecipientGroupId", message.RecipientGroupId));
            paramz.Add(new SqlParameter("messageDirectionEnumID", message.MessageDirectionEnumID)); // sent, received
            paramz.Add(new SqlParameter("messageSubject", message.Subject));
            paramz.Add(new SqlParameter("messageBody", message.Body));
            paramz.Add(new SqlParameter("messageDate", message.MessageDate));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return reader.GetInt32(0); // new id
            };

            var list = this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }


        /// <summary>
        /// Deletes a message
        /// </summary>
        public void DeleteMessage(int messageID)
        {
            var proc = "DeleteMessage";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("messageId", messageID));

            this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
        }


        public void SaveEmailConfiguration(EmailConfiguration emailConfiguration)
        {
            var proc = "SaveEmailConfigurations";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("churchId", emailConfiguration.ChurchId));
            paramz.Add(new SqlParameter("emailConfigProfileId", emailConfiguration.EmailConfigProfileId));
            paramz.Add(new SqlParameter("userName", emailConfiguration.UserName));
            paramz.Add(new SqlParameter("password", emailConfiguration.Password));

            this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
        }


        public EmailConfigurationProfile SaveEmailConfigurationProfiles(EmailConfigurationProfile emailConfiguration)
        {
            var proc = "SaveEmailConfigurationProfiles";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("Name", emailConfiguration.Name));
            paramz.Add(new SqlParameter("Server", emailConfiguration.Server));
            paramz.Add(new SqlParameter("Port", emailConfiguration.Port));

            Func<SqlDataReader, int> readFx = (reader) =>
            {
                return reader.ValueOrDefault<int>("Id");
            };

            var list = this.executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            emailConfiguration.Id = list.FirstOrDefault();

            return emailConfiguration;
        }

        public List<EmailConfigurationProfile> GetEmailConfigurationProfiles()
        {
            var proc = "GetEmailConfigurationProfiles";

            var paramz = new List<SqlParameter>();

            Func<SqlDataReader, EmailConfigurationProfile> readFx = (reader) =>
            {
                var item = new EmailConfigurationProfile();
                item.Id = reader.ValueOrDefault<int>("Id");
                item.Name = reader.ValueOrDefault<string>("Name");
                item.Server = reader.ValueOrDefault<string>("Server");
                item.Port = reader.ValueOrDefault<int>("Port");
                return item;
            };

            var list = this.executor.ExecuteSql<EmailConfigurationProfile>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }

        public SmsConfiguration GetSmsConfiguration(int recipientGroupId)
        {
            var proc = "GetSmsTokens";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("recipientGroupId", recipientGroupId));

            Func<SqlDataReader, SmsConfiguration> readFx = (reader) =>
            {
                var item = new SmsConfiguration();
                item.Sid = reader.ValueOrDefault<string>("AccountSid");
                item.Token = reader.ValueOrDefault<string>("AccountToken");
                item.PhoneNumber = reader.ValueOrDefault<string>("PhoneNumber");
                return item;
            };

            var list = this.executor.ExecuteSql<SmsConfiguration>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.FirstOrDefault();
        }


        public List<IMessage> GetMessages(int recipientGroupId, int maxReturnRows)
        {
            var proc = "GetMessages";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("messageRecipientGroupId", recipientGroupId));
            paramz.Add(new SqlParameter("maxrows", maxReturnRows));

            Func<SqlDataReader, IMessage> readFx = (reader) =>
            {
                var item = new Message();
                item.Id = reader.ValueOrDefault<long>("id");
                item.RecipientGroupId = reader.ValueOrDefault<int>("MessageRecipientGroupId");
                item.MessageDirectionEnumID = reader.ValueOrDefault<int>("MessageDirectionEnumID");
                item.Subject = reader.ValueOrDefault<string>("MessageSubject");
                item.Body = reader.ValueOrDefault<string>("MessageBody");
                item.MessageDate = reader.ValueOrDefault<DateTimeOffset>("MessageDate");
                return item;
            };

            var list = this.executor.ExecuteSql<IMessage>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }
    }
}
