using MailClient.Application.Interfaces;
using MailClient.Application.Services;
using MailClient.Domain.Repositories;
using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Connection;
using MailClient.Infrastructure.Interfaces;
using MailClient.Infrastructure.Publishers;
using MailClient.Infrastructure.Repositories;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MailClient.IoC
{
    public static class IoC
    {
        public static IHostApplicationBuilder ConfigureApp(this IHostApplicationBuilder builder)
        {
            builder.Services.AddConfigurationRabbitMq(builder.Configuration);
            builder.Services.AddMongoDBConfiguration(builder.Configuration);
            builder.Services.AddInfrastructure();
            return builder;
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDBConnection, MongoDBConnection>();
            services.AddScoped<IRepositoryEmail, RepositoryEmail>();
            services.AddScoped<IPublisher, RabbitMqPublisher>();
            services.AddScoped<ISmtpClient, SmtpClient>();
            services.AddTransient<IImapClient, ImapClient>();
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
