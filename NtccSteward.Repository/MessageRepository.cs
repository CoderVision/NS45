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
        List<ICorrespondence> GetCorresondence(int churchID, int messageTypeEnumID, int maxReturnRows);
        List<IRecipientGroup> GetGroups(int churchID);
        List<IMessage> GetMessages(int correspondenceID, int messageTypeEnumID, int maxReturnRows);
        List<RecipientGroup> GetRecipientGroups(int churchId);
        int SaveMessage(IMessage message);
        int SaveRecient(int recipientId, int recipientGroupId);
        int SaveRecipientGroup(RecipientGroup group);
    }

    public class MessageRepository : NtccSteward.Repository.Repository, IMessageRepository
    {
        private readonly ISqlCmdExecutor _executor;

        public MessageRepository(string connectionString)
        {
            ConnectionString = connectionString;
            _executor = new SqlCmdExecutor(connectionString);
        }


        public List<RecipientGroup> GetRecipientGroups(int churchId)
        {
            var list = new List<RecipientGroup>();

            var proc = "GetMessageRecipientGroups";

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(proc, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ChurchId", churchId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var group = new RecipientGroup();
                        group.Id = reader.ValueOrDefault<int>("Id");
                        group.ChurchId = reader.ValueOrDefault<int>("ChurchId");
                        group.Name = reader["Name"].ToString();
                        group.Description = reader["Description"].ToString();

                        list.Add(group);
                    }
                }
            }

            return list;
        }


    /// <summary>
    /// Saves the group, returns the ID.
    /// </summary>
    /// <returns>The ID of the group (new if insert, same if update)</returns>
    public int SaveRecipientGroup(RecipientGroup group)
    {
        var proc = "MessageRecipientGroup_Merge";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("MessageRecipientGroupId", group.ID));
        paramz.Add(new SqlParameter("GroupName", group.Name));
        paramz.Add(new SqlParameter("churchIdentityId", group.ChurchId));

        Func<SqlDataReader, int> readFx = (reader) =>
        {
            return reader.GetInt32(0); // new id
            };

        var list = _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, readFx);

        return list.First();
    }


    public void DeleteRecipientGroup(int recipientGroupId)
    {
        var proc = "MessageRecipientGroup_Delete";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("MessageRecipientGroupId", recipientGroupId));

        _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
    }


    /// <summary>
    /// Saves the recipient, returns the ID.
    /// </summary>
    /// <returns>The ID of the recipient (new if insert, same if update)</returns>
    public int SaveRecient(int recipientId, int recipientGroupId)
    {
        var proc = "MessageRecipient_Insert";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("IdentityId", recipientId));
        paramz.Add(new SqlParameter("MessageRecipientGroupId", recipientGroupId));

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
        var proc = "MessageRecipient_Delete";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("MessageRecipientId", recipientId));

        _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
    }


    /// <summary>
    /// Saves a message by inserting a new record in the database.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>The ID of the new message</returns>
    public int SaveMessage(IMessage message)
    {
        var proc = "Messages_Insert";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("personIdentityID", message.IdentityID));
        paramz.Add(new SqlParameter("messageTypeEnumID", message.MessageTypeEnumID)); // text, email
        paramz.Add(new SqlParameter("messageDirectionEnumID", message.MessageDirectionEnumID)); // sent, received
        paramz.Add(new SqlParameter("messageSubject", message.Subject));
        paramz.Add(new SqlParameter("messageBody", message.Body));

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
        var proc = "Message_Delete";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("messageId", messageID));

        _executor.ExecuteSql<int>(proc, CommandType.StoredProcedure, paramz, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="churchID"></param>
    /// <param name="messageTypeEnumID">Text, Email</param>
    /// <returns></returns>
    public List<IMessage> GetMessages(int correspondenceID, int messageTypeEnumID, int maxReturnRows)
    {
        var proc = "Message_SelectByIdentityID";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("identityID", correspondenceID));
        paramz.Add(new SqlParameter("messageTypeEnumID", messageTypeEnumID)); // text, email
        paramz.Add(new SqlParameter("maxReturnRows", maxReturnRows));

        Func<SqlDataReader, IMessage> readFx = (reader) =>
        {
            var item = new Message();
            item.MessageID = reader.ValueOrDefault<long>("MessageId");
            item.IdentityID = reader.ValueOrDefault<int>("IdentityID");
            item.IdentityName = reader.ValueOrDefault<string>("Name");
            item.MessageTypeEnumID = reader.ValueOrDefault<int>("MessageTypeEnumID");
            item.MessageDirectionEnumID = reader.ValueOrDefault<int>("MessageDirectionEnumID");
            item.MessageDate = reader.ValueOrDefault<DateTime>("genDate");
            item.Subject = reader.ValueOrDefault<string>("MessageSubject");
            item.Body = reader.ValueOrDefault<string>("MessageBody");
            return item;
        };

        var list = _executor.ExecuteSql<IMessage>(proc, CommandType.StoredProcedure, paramz, readFx);

        return list;
    }

    /// <summary>
    /// Gets message groups, does not include recipients
    /// </summary>
    /// <param name="churchID">churchID</param>
    /// <returns></returns>
    public List<IRecipientGroup> GetGroups(int churchID)
    {
        var proc = "GetMessageRecipientGroups";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("churchId", churchID));

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
            
            return item;
        };

        var list = _executor.ExecuteSql<IRecipientGroup>(proc, CommandType.StoredProcedure, paramz, readFx);

        return list;
    }



    public List<IMessage> GetMessages(int recipientGroupId, int maxReturnRows)
    {
        var proc = "GetMessages";

        var paramz = new List<SqlParameter>();
        paramz.Add(new SqlParameter("recipientGroupId", recipientGroupId));
        paramz.Add(new SqlParameter("maxRowsToReturn", maxReturnRows));

        Func<SqlDataReader, IMessage> readFx = (reader) =>
        {
            var item = new Message();
            item.Id = reader.ValueOrDefault<int>("id");
            item.RecipientId = reader.ValueOrDefault<int>("RecipientId");
            item.MessageTypeEnumID = reader.ValueOrDefault<int>("MessageTypeEnumID");
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
