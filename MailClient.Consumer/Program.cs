﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MailClient.Consumer.Configuration;
using MailClient.Services;

namespace MailClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var consumerService = host.Services.GetRequiredService<ConsumerEmailImapService>();

            await consumerService.ExecuteAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
        {
            services.Configure<RabbitMqConfiguration>(context.Configuration.GetSection("RabbitMqConfiguration"));
            services.AddSingleton<ConsumerEmailImapService>();
        });
    }
}
