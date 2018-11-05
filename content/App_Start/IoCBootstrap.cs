using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkerOracleTemplate.Business;
using WorkerOracleTemplate.Infra.Database;
using WorkerOracleTemplate.Infra.Log;
using WorkerOracleTemplate.Models.Options;
using WorkerOracleTemplate.Repositories;
using WorkerOracleTemplate.Services;

namespace WorkerOracleTemplate
{
    public static class IoCBootstrap
    {
        public static IServiceProvider Configure(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddLogging (builder => {
                builder.Services.Configure<LogstashOptions>(configuration.GetSection("Logstash"));

                builder.AddConfiguration(configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddLogstash();
            });

            services.AddOptions(configuration);
            services.AddDatabase(configuration);
            services.AddRepositories();
            services.AddBusiness();
            services.AddServices();

            return services.BuildServiceProvider();
        }

        private static void AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WorkerOptions>(configuration.GetSection("Worker"));
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<TesteRepository>();
        }

        private static void AddBusiness(this IServiceCollection services)
        {
            services.AddTransient<TesteBusiness>();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<TesteService>();
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionOptions>(options => 
            {
                options.DataSource = configuration.GetValue<string>("DB_DATASOURCE");
                options.Username = configuration.GetValue<string>("DB_USERNAME");
                options.Password = configuration.GetValue<string>("DB_PASSWORD");

                var isPooling = configuration.GetValue<bool>("DB_POOLING");

                if (isPooling)
                {
                    options.Pooling = new ConnectionOptions.ConnectionPoolingOptions
                    {
                        Pooling = true,
                        MaxSize = configuration.GetValue<int>("DB_POOLING_MAX_SIZE"),
                        MinSize = configuration.GetValue<int>("DB_POOLING_MIN_SIZE")
                    };
                }
            });

            services.AddTransient<Connection>();
        }
    }
}