using MailClient.Infrastructure.Configuration;
using RabbitMQ.Client;

namespace MailClient.Infrastructure.Factory
{
    public static class RabbitMqFactory
    {
        public static ConnectionFactory CreateConnection(RabbitMqConfiguration configuration)
        {
            return new ConnectionFactory { HostName = configuration.Host, RequestedHeartbeat = TimeSpan.FromSeconds(30), SocketReadTimeout = TimeSpan.FromSeconds(30) };
        }
    }
}
