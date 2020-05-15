using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Forum.DataAccess.Services
{
    public class EmailSender : IEmailSender
    {
        public IConfiguration _config { get; }
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string myGmailAddress = _config.GetValue<string>("EmailSend:Email");
            string appSpecificPassword = _config.GetValue<string>("EmailSend:Password");

            MailMessage msg = new MailMessage();
            msg.Sender = new MailAddress(myGmailAddress, "Administrator GeekTZ");
            msg.From = new MailAddress(myGmailAddress, "Administrator GeekTZ");
            msg.To.Add(new MailAddress(email, "User"));
            msg.Subject = subject;
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient(_config.GetValue<string>("EmailSend:Client"));
            client.EnableSsl = _config.GetValue<bool>("EmailSend:SSl");
            client.Port = _config.GetValue<int>("EmailSend:Port");
            client.Credentials = new NetworkCredential(myGmailAddress, appSpecificPassword);

            return client.SendMailAsync(msg);
        }
    }
}
