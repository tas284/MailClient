using MailClient.API.InputModel;
using MailClient.API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace MailClient.API.Services
{
    public class EmailSmtpService : IEmailSmtpService
    {
        public EmailSmtpService() { }

        public Task<string> Send(SendEmailInputModel input)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto);

                    client.Authenticate(input.User, input.Password);

                    var message = CreateMessage(input);

                    client.Send(message);

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    return Task.FromResult($"Error to send email: {ex.Message}.");
                }
                
            }

            return Task.FromResult("Email sent successfull!");
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
