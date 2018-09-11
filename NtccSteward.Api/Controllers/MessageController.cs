
using System;
using System.Threading.Tasks;
using NtccSteward.Core.Framework;
using System.Net.Mail;
using NtccSteward.Core.Models.Message;
using NtccSteward.Core.Models.Common.Parameters;
using System.Web.Http;
using NtccSteward.Repository;
using NtccSteward.Api.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace NtccSteward.Api.Controllers
{
    [Authorize]
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
        public async Task<IHttpActionResult> GetMetadata() {

            var userId = TokenIdentityHelper.GetOwnerIdFromToken();

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

            var config = this.repo.GetSmsConfiguration(message.RecipientGroupId);

            //// these are test credentials.  These need to be stored in the database, because it will be different for each church.
            //var accountSID = "ACbf84ef7f4c8900348b5fbbd4f7c519f2";
            //var authToken = "481094fec8f4f91654afcb7c03b5e259";
            ////sender = "+15005550006"; this is Gary's twilio phone number for sending sms.

            //config.Sid = "AC743a79079b3df525e74b71ba2498ba7b"; //= "PN95fb71e231746fad3c988398976f2a42";  // sid for Gary's number:  "+12536552307
            //config.Token = "a14f98f62a64d3e00d9a2472f7caf895";
            //config.PhoneNumber = "+12536552307";  // Gary's number that he bought - $1 a month - for dev only, canncel ASAP

            var client = new Twilio.TwilioRestClient(config.Sid, config.Token);

            var recipients = this.repo.GetGroupRecipients(message.RecipientGroupId);
            foreach (var recipient in recipients)
            {
                await Task.Run(() => {
                    var sms = client.SendMessage(config.PhoneNumber, recipient.Address, message.Body);
                });
            }

            return Ok(message);
        }

        [AllowAnonymous]
        [Route("message/receiveSms")]
        [HttpPost()]
        public async void ReceiveInboundMessage()
        {
            var smsJson = await this.Request.Content.ReadAsStringAsync();

            var sms = new System.Net.Http.Formatting.FormDataCollection(smsJson).ReadAs<TwilioIncomingSms>();

            this.logger.LogInfo(LogLevel.Information, $"Text message received for {sms.To}, coming from: {sms.From}", $"Text message received for {sms.Body}", 0);

            // TODO:  
            //  Save to Database using the To message to find out what church phone number it's being sent to
            //  Forward the message to the client using Web Sockets if the user is online
        }


        [Route("message/list")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMessages(int recipientGroupId, int maxRows)
        {
            var messages = this.repo.GetMessages(recipientGroupId, maxRows);

            return Ok(messages);
        }

        [Route("message")]
        [HttpPost()]
        public async Task<IHttpActionResult> SaveMessage([FromBody] Message message)
        {
            var messageId = this.repo.SaveMessage(message);
            message.Id = messageId;

            return Ok(message);
        }

        [Route("message")]
        [HttpDelete()]
        public async Task<IHttpActionResult> DeleteMessage(int id)
        {
            this.repo.DeleteMessage(id);

            return Ok();
        }

        [Route("message/recipients")]
        [HttpGet()]
        public async Task<IHttpActionResult> GetRecipients(int churchId, int messageTypeEnumId, string criteria)
        {
            var recipients = this.repo.GetRecipients(churchId, messageTypeEnumId, criteria);

            return Ok(recipients);
        }

        [Route("message/recipientGroup/{id}")]
        [HttpGet()]
        public async Task<IHttpActionResult> GetRecipientGroup(int id)
        {
            var group = this.repo.GetGroupRecipients(id);

            return Ok(group);
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
            if (string.IsNullOrEmpty(recipientGroup.Description))
                recipientGroup.Description = recipientGroup.Name;

            // delete recipients that are no longer in the list
            var dbRecipients = this.repo.GetGroupRecipients(recipientGroup.Id);
            var deletedRecipients = dbRecipients.Where(dbr => !recipientGroup.Recipients.Any(r => r.Id == dbr.Id));
            foreach (var r in deletedRecipients)
                this.repo.DeleteRecipient(r.Id);

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

        [Route("message/recipientGroups/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRecipientGroup(int id)
        {
            this.repo.DeleteRecipientGroup(id);

            return Ok();
        }
    }
}
