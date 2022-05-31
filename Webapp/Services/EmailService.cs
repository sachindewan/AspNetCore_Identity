using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Webapp.Settings;

namespace Webapp.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<EmailSetting> _options;
        public EmailService(IConfiguration configuration,IOptions<EmailSetting> options)
        {
            _configuration = configuration;
            _options = options;
        }

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);
            using var emailClient = new SmtpClient("smtp-relay.sendinblue.com", 587);
            emailClient.Credentials = new NetworkCredential("sachindewan12@gmail.com", "wfPjTZ6Um3xWRpkY");
            await emailClient.SendMailAsync(message);
        }
    }
}
