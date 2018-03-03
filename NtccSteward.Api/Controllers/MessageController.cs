﻿
using System;
using System.Threading.Tasks;
using NtccSteward.Core.Framework;
using System.Net.Mail;
using NtccSteward.Core.Models.Message;
using NtccSteward.Core.Models.Common.Parameters;
using System.Web.Http;
using NtccSteward.Repository;

namespace NtccSteward.Api.Controllers
{
 //   [Authorize]
    public class MessageController : ApiController
    {
        //private readonly SmsSettings _smsSettings;
        private Repository.ILogger logger;
        private readonly IMessageRepository repo;

        //, IOptions<SmsSettings> smsSettings
        public MessageController(IMessageRepository repository, ILogger logger)
        {
            //_smsSettings = smsSettings.Value;
            this.logger = logger;
            this.repo = repository;
        }

        [Route("message/metadata")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMetadata(int userId) {

            var metadata = this.repo.GetMetadata(userId);

            var result = new {
                churches = metadata.Churches,
                enums = metadata.Enums
            };

            return Ok(result);
        }

        [Route("message/email")]
        [HttpPost()]
        public async Task<IHttpActionResult> SendEmail(Message message)
        {
            var id = this.repo.SaveMessage(message);

            message.Id = id;

            return Ok(message);

            // need to get email information from db for church
            // select the top 1 email by churchId order by preferred desc

            // See:  http://www.codeproject.com/Tips/520998/Send-Email-from-Yahoo-GMail-Hotmail-Csharp
            var sender = "gary.a.lima@gmail.com";
            var password = "my password";
            var host = "smtp.gmail.com";
            var port = 587;
            var useSsl = true;

            /*
             * Gmail    smtp.gmail.com   587    SSL
             * Hotmail  smtp.live.com   587     SSL
             * Yahoo!   smtp.mail.yahoo.com 587 SSL
             */

            //var mailMessage = new MailMessage(sender, recipient, subject, body);

            ////http://stackoverflow.com/questions/29465096/how-to-send-an-e-mail-with-c-sharp-through-gmail
            ////
            //// abstract to separate class.
            //var smtpClient = new SmtpClient();
            //smtpClient.Host = host;
            //smtpClient.Port = port;
            //smtpClient.EnableSsl = useSsl;
            //smtpClient.Credentials = new System.Net.NetworkCredential("myemail@gmail.com", "mypassword");
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtpClient.UseDefaultCredentials = false;

            //try
            //{
            //    smtpClient.Send(mailMessage);

            //    return "success";
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
        }


        [Route("message/sendSms")]
        [HttpPost()]
        public async Task<IHttpActionResult> SendSms(Message message)
        {
            //// select the top 1 phone number by churchId order by preferred desc

            //// See this link for Twilio getting started:
            //https://www.twilio.com/docs/quickstart/csharp/sms/sending-via-rest

            var id = this.repo.SaveMessage(message);

            message.Id = id;


            //// these are test credentials.  These need to be stored in the database, because it will be different for each church.
            //var accountSID = "ACbf84ef7f4c8900348b5fbbd4f7c519f2";
            //var authToken = "481094fec8f4f91654afcb7c03b5e259";
            ////sender = "+15005550006"; this is Gary's twilio phone number for sending sms.

            //if (!string.IsNullOrWhiteSpace(subject))
            //    body = $"({subject}) {body}";

            //var client = new Twilio.TwilioRestClient(accountSID, authToken);
            //var sms = client.SendMessage("+15005550006", recipient, body);

            return Ok(message);
        }


        [Route("message/receiveSms")]
        [HttpPost()]
        public void ReceiveInboundMessage(string msisdn, string to, string messageId, string text, string type, string message)
        {
            //https://docs.nexmo.com/messaging/sms-api/api-reference#inbound

            this.logger.LogInfo(LogLevel.Information, $"Text message received for {to}", $"Text message received for {to}", 0);
            // example of inbound message:  
            //  ?msisdn=19150000001&to=12108054321
            //  &messageId = 000000FFFB0356D1 & text = This +is+ an + inbound + message
            //  & type = text & message - timestamp = 2012 - 08 - 19 + 20 % 3A38 % 3A23
            return;
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetMessages(int recipientGroupId, int maxRows)
        {
            var messages = this.repo.GetMessages(recipientGroupId, maxRows);

            return Ok(messages);
        }

        [HttpPost()]
        public async Task<IHttpActionResult> SaveMessage([FromBody] Message message)
        {
            var messageId = this.repo.SaveMessage(message);
            message.Id = messageId;

            return Ok(message);
        }

        [HttpDelete()]
        public async Task<IHttpActionResult> DeleteMessage(int id)
        {
            this.repo.DeleteMessage(id);

            return Ok();
        }

        [Route("message/recipientGroups")]
        [HttpGet()]
        public async Task<IHttpActionResult> GetRecipientGroups(int churchId, int messageTypeEnumId)
        {
            var groups = this.repo.GetRecipientGroups(churchId, messageTypeEnumId);

            return Ok(groups);
        }

        [Route("message/recipientGroups")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveRecipientGroups(RecipientGroup recipientGroup)
        {
            var id = this.repo.SaveRecipientGroup(recipientGroup);
            recipientGroup.Id = id;

            foreach (var r in recipientGroup.Recipients)
            {
                r.MessageRecipientGroupId = recipientGroup.Id;

                this.repo.SaveRecipient(r);

                var recipient = this.repo.GetRecipient(r.ContactInfoId, recipientGroup.Id);
                r.Id = recipient.Id;
                r.Name = recipient.Name;
                r.Address = recipient.Address;
            }

            return Ok(recipientGroup);
        }

        [Route("message/recipientGroups")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRecipientGroup(int id)
        {
            this.repo.DeleteRecipientGroup(id);

            return Ok();
        }
    }
}
