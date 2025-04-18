﻿using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using MailClient.Application.Specification;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailClient.Application.Services
{
    public class EmailSmtpService : IEmailSmtpService
    {
        private readonly ILogger<EmailSmtpService> _logger;
        private readonly ISmtpClient _smtpClient;
        private readonly ISpecification<SendEmailInputModel> _spec;

        public EmailSmtpService(ILogger<EmailSmtpService> logger, ISmtpClient smtpClient)
        {
            _logger = logger;
            _smtpClient = smtpClient;
            _spec = new IsValidSendEmailInputModelSpec();
        }

        public string Send(SendEmailInputModel input)
        {
            if (!_spec.IsSatisfiedBy(input)) throw new ArgumentException(input.Validations);

            var result = SendEmail(input);
            return result;
        }

        private string SendEmail(SendEmailInputModel input)
        {
            var result = string.Empty;
            using (_smtpClient)
            {
                try
                {
                    _smtpClient.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto);

                    _smtpClient.Authenticate(input.User, input.Password);
                    _logger.LogInformation($"Email authenticated on: {input.SmtpAddress}:{input.SmtpPort}");

                    var message = CreateMessage(input);

                    var status = _smtpClient.Send(message);
                    _logger.LogInformation($"Result MailKit Send [{status}].");

                    result = $"Email send succesfully to {message.To}.";
                    _logger.LogInformation(result);

                    _smtpClient.Disconnect(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return ex.Message;
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
            
            return CreateBody(message, input);
        }

        private MimeMessage CreateBody(MimeMessage message, SendEmailInputModel input)
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
            return message;
        }
    }
}
