namespace MailClient.Application.Configuration
{
    public class RabbitMqConfiguration
    {
        public string Host { get; set; }
        public string QueueMail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
