using MailClient.API.Configuration;
using MailClient.API.Interfaces;
using MailClient.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailClient.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IEmailImapService, EmailImapService>();
            services.AddScoped<IEmailSmtpService, EmailSmtpService>();
            return services;
        }

        public static IConfiguration AddConfigurationRabbitMq(this IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMqConfiguration"));
            return configuration;
        }
    }
}
