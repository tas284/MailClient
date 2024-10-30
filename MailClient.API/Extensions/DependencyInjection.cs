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
using Microsoft.Extensions.Hosting;

namespace MailClient.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IHostApplicationBuilder Setup(this IHostApplicationBuilder builder)
        {
            builder.Services.AddConfigurationRabbitMq(builder.Configuration);
            builder.Services.AddMongoDBConfiguration(builder.Configuration);
            builder.Services.AddInfrastructure();
            return builder;
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IConnection, MongoDBConnection>();
            services.AddScoped<IRepositoryEmail, RepositoryEmail>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailImapService, EmailImapService>();
            services.AddScoped<IEmailSmtpService, EmailSmtpService>();
            return services;
        }

        private static IServiceCollection AddConfigurationRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMqConfiguration"));
        }

        private static IServiceCollection AddMongoDBConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<MongoDBConfiguration>(configuration.GetSection("MongoDBConfiguration"));
        }
    }
}
