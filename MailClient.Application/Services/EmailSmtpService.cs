using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailClient.Application.Services
{
    public class EmailSmtpService : IEmailSmtpService
    {
        private readonly ILogger<EmailSmtpService> _logger;

        public EmailSmtpService(ILogger<EmailSmtpService> logger)
        {
            _logger = logger;
        }

        public string Send(SendEmailInputModel input)
        {
            string result = string.Empty;
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto);

                    client.Authenticate(input.User, input.Password);
                    _logger.LogInformation($"Email authenticated on: {input.SmtpAddress}:{input.SmtpPort}");

                    var message = CreateMessage(input);

                    var status = client.Send(message);
                    _logger.LogInformation($"Result MailKit Send [{status}].");
                    
                    result = $"Email send succesfully to {message.To}.";
                    _logger.LogInformation(result);

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    var error = $"Error to send email: {ex.Message}.";
                    _logger.LogError(error);
                    return error;
                }
                
            }

            return result;
        }

        private MimeMessage CreateMessage(SendEmailInputModel input)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(input.FromName, input.FromEmail));
            message.To.Add(new MailboxAddress(input.ToName, input.ToEmail));
            message.Subject = input.Subject;

            CreateBody(input, message);

            return message;
        }

        private void CreateBody(SendEmailInputModel input, MimeMessage message)
        {
            if (!string.IsNullOrEmpty(input.BodyHtml))
            {
                var builder = new BodyBuilder();
                builder.TextBody = input.Body;
                builder.HtmlBody = input.BodyHtml;
                message.Body = builder.ToMessageBody();
            }
            else
            {
                message.Body = new TextPart("plain") { Text = input.Body };
            }
        }
    }
}
