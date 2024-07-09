using MailClient.Configuration;
using MailClient.Interfaces;
using MailClient.Services;

namespace MailClient.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IEmailImapService, EmailImapService>();
            return services;
        }

        public static IConfiguration AddConfigurationRabbitMq(this IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMqConfiguration"));
            return configuration;
        }
    }
}
