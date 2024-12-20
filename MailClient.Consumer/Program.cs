﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MailClient.Services;
using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Connection;
using MailClient.Infrastructure.Interfaces;
using MailClient.Domain.Repositories;
using MailClient.Consumer.Interfaces;
using MailClient.Infrastructure.Repositories;

namespace MailClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var consumerService = host.Services.GetRequiredService<IConsumerEmailImapService>();

            await consumerService.ExecuteAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
        {
            services.Configure<RabbitMqConfiguration>(context.Configuration.GetSection("RabbitMqConfiguration"));
            services.Configure<MongoDBConfiguration>(context.Configuration.GetSection("MongoDBConfiguration"));
            services.AddSingleton<IDBConnection, MongoDBConnection>();
            services.AddScoped<IRepositoryEmail, RepositoryEmail>();
            services.AddSingleton<IConsumerEmailImapService, ConsumerEmailImapService>();
        });
    }
}
