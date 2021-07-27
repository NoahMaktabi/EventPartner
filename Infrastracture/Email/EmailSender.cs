using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastracture.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string userEmail, string emailSubject, string msg)
        {
            var client = new SendGridClient(_config["Sendgrid:Key"]);
            var message = new SendGridMessage
            {
                From = new EmailAddress("mctabi@me.com", _config["SendGrid:User"]),
                Subject = emailSubject,
                HtmlContent = msg
            };
            message.AddTo(new EmailAddress(userEmail));
            message.SetClickTracking(false, false);

            await client.SendEmailAsync(message);
        }
    }
}
