using NtccSteward.Repository.Framework;
using NtccSteward.Core.Interfaces.Messages;
using NtccSteward.Core.Models.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
    }

    public class MessageRepository : NtccSteward.Repository.Repository, IMessageRepository
    {
        private readonly ISqlCmdExecutor _executor;

        public MessageRepository(string connectionString)
        {
            ConnectionString = connectionString;
            _executor = new SqlCmdExecutor(connectionString);
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

            var list = _executor.ExecuteSql<IRecipientGroup>(proc, CommandType.StoredProcedure, paramz, readFx);

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

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }


        public void DeleteRecipientGroup(int recipientGroupId)
        {
            var proc = "DeleteMessageRecipientGroup";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("MessageRecipientGroupId", recipientGroupId));

            _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
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

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list.First();
        }

        /// <summary>
        /// Deletes a message recipient, removing it from the group
        /// </summary>
        public void DeleteRecipient(int recipientId)
        {
            var proc = "DeleteMessageRecipient";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("Id", recipientId));

            _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
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

            var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

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

            _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
        }



        public List<IMessage> GetMessages(int recipientGroupId, int maxReturnRows)
        {
            var proc = "GetMessages";

            var paramz = new List<SqlParameter>();
            paramz.Add(new SqlParameter("messageRecipientGroupId", recipientGroupId));
            paramz.Add(new SqlParameter("maxRowsToReturn", maxReturnRows));

            Func<SqlDataReader, IMessage> readFx = (reader) =>
            {
                var item = new Message();
                item.Id = reader.ValueOrDefault<int>("id");
                item.RecipientGroupId = reader.ValueOrDefault<int>("MessageRecipientGroupId");
                item.MessageDirectionEnumID = reader.ValueOrDefault<int>("MessageDirectionEnumID");
                item.Subject = reader.ValueOrDefault<string>("MessageSubject");
                item.Body = reader.ValueOrDefault<string>("MessageBody");
                item.MessageDate = reader.ValueOrDefault<DateTimeOffset>("MessageDate");
                return item;
            };

            var list = _executor.ExecuteSql<IMessage>(proc, CommandType.StoredProcedure, paramz, readFx);

            return list;
        }
    }
}
