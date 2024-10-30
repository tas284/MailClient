using MailClient.Application.Configuration;
using MailClient.Application.Interfaces;
using MailClient.Application.Services;
using MailClient.Domain.Interfaces;
using MailClient.Domain.Repositories;
using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Connection;
using MailClient.Infrastructure.Repostitories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailClient.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IConnection, MongoDBConnection>();
            services.AddScoped<IRepositoryEmail, RepositoryEmail>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailImapService, EmailImapService>();
            services.AddScoped<IEmailSmtpService, EmailSmtpService>();
            return services;
        }

        public static IConfiguration AddConfigurationRabbitMq(this IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMqConfiguration"));
            return configuration;
        }

        public static IConfiguration AddMongoDBConfiguration(this IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<MongoDBConfiguration>(configuration.GetSection("MongoDBConfiguration"));
            return configuration;
        }
    }
}
