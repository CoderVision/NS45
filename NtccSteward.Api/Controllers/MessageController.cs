

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
    public class MessageController : ApiController
    {
        //private readonly SmsSettings _smsSettings;
        private Repository.ILogger _logger;
        private readonly IMessageRepository _messageRepo;

        //, IOptions<SmsSettings> smsSettings
        public MessageController(IMessageRepository repository, ILogger logger)
        {
            //_smsSettings = smsSettings.Value;
            _logger = logger;
            _messageRepo = repository;
        }

        [Route("message/email")]
        [HttpPost()]
        public async Task<string> SendEmail(int churchId, string recipient, string subject, string body)
        {
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

            var mailMessage = new MailMessage(sender, recipient, subject, body);

            //http://stackoverflow.com/questions/29465096/how-to-send-an-e-mail-with-c-sharp-through-gmail
            //
            // abstract to separate class.
            var smtpClient = new SmtpClient();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = useSsl;
            smtpClient.Credentials = new System.Net.NetworkCredential("myemail@gmail.com", "mypassword");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;

            try
            {
                smtpClient.Send(mailMessage);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [Route("message/sendSms")]
        [HttpPost()]
        public async Task<string> SendSms(Message message)
        {
            //// select the top 1 phone number by churchId order by preferred desc

            //// See this link for Twilio getting started:
            //https://www.twilio.com/docs/quickstart/csharp/sms/sending-via-rest

            //// these are test credentials.  These need to be stored in the database, because it will be different for each church.
            //var accountSID = "ACbf84ef7f4c8900348b5fbbd4f7c519f2";
            //var authToken = "481094fec8f4f91654afcb7c03b5e259";
            ////sender = "+15005550006"; this is Gary's twilio phone number for sending sms.

            //if (!string.IsNullOrWhiteSpace(subject))
            //    body = $"({subject}) {body}";

            //var client = new Twilio.TwilioRestClient(accountSID, authToken);
            //var sms = client.SendMessage("+15005550006", recipient, body);

            return "success";
        }


        [Route("message/receiveSms")]
        [HttpPost()]
        public void ReceiveInboundMessage(string msisdn, string to, string messageId, string text, string type, string message)
        {
            //https://docs.nexmo.com/messaging/sms-api/api-reference#inbound

            _logger.LogInfo(LogLevel.Information, $"Text message received for {to}", $"Text message received for {to}", 0);
            // example of inbound message:  
            //  ?msisdn=19150000001&to=12108054321
            //  &messageId = 000000FFFB0356D1 & text = This +is+ an + inbound + message
            //  & type = text & message - timestamp = 2012 - 08 - 19 + 20 % 3A38 % 3A23
            return;
        }


        //[HttpPost("GetMessages")]
        //public IActionResult GetMessages([FromBody] GetMessagesParameter getMessagesParameter)
        //{
        //    var list = _messageRepo.GetMessages(getMessagesParameter.CorrespondenceID, getMessagesParameter.MessageTypeEnumID, getMessagesParameter.MaxReturnRows);

        //    return Json(list);
        //}

        //[HttpPost("SaveMessage")]
        //public IActionResult SaveMessage([FromBody] Message message)
        //{
        //    var messageID = _messageRepo.SaveMessage(message);

        //    return Ok(messageID);
        //}

        //[HttpPost("GetCorrespondence")]
        //public IActionResult GetCorrespondence([FromBody] GetCorrespondenceParameter param)
        //{
        //    var list = _messageRepo.GetCorresondence(param.ChurchID, param.MessageTypeEnumID, param.MaxReturnRows);

        //    return Json(list);
        //}

        //[HttpPost("GetGroups")]
        //public IActionResult GetGroups([FromBody] int churchID)
        //{
        //    var list = _messageRepo.GetGroups(churchID);

        //    return Json(list);
        //}
    }
}
