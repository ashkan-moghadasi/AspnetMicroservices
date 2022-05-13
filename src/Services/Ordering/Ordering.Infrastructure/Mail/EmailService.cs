using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService :IEmailService
    {
        private readonly EmailSettings _mailSettings;
        private readonly ILogger<EmailSettings> _logger;

        public EmailService(IOptions<EmailSettings> mailSettings, ILogger<EmailSettings> logger)
        {
            _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendMail(Email email)
        {
            var client = new SendGridClient(_mailSettings.ApiKey);
            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var from = new EmailAddress(_mailSettings.FromAddress, _mailSettings.FromName);
            var sendGridMessage=MailHelper.CreateSingleEmail(from, to, subject, email.Body, "");
            var response=await client.SendEmailAsync(sendGridMessage);
            _logger.LogInformation($"Mail Send To {to}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Mail Received Successfully");
                return true;
            }
            _logger.LogInformation($"Mail Send To {to} Failed");
            return false;
        }
    }
}