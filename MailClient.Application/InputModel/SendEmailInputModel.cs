﻿using System.Text.Json.Serialization;

namespace MailClient.Application.InputModel
{
    public class SendEmailInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyHtml { get; set; }

        [JsonIgnore]
        public string Validations { get; set; }
    }
}
