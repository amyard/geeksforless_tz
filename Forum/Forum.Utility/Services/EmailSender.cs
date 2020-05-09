using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Forum.Utility.Services
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
            string myGmailAddress = "geektz3000@gmail.com";
            string appSpecificPassword = "mclpfdtdzcnopwva";
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.EnableSsl = true;
            client.Port = 587;
            client.Credentials = new NetworkCredential(myGmailAddress, appSpecificPassword);

            MailMessage msg = new MailMessage();
            msg.Sender = new MailAddress(myGmailAddress, "Administrator GeekTZ");
            msg.From = new MailAddress(myGmailAddress, "Administrator GeekTZ");
            msg.To.Add(new MailAddress(email, "Recipient Number 1"));
            msg.Subject = subject;
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;
            return client.SendMailAsync(msg);
        }
    }
}
