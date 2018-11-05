using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using WorkerOracleTemplate.Services;

namespace WorkerOracleTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceRunner<TesteService>.Run(config =>
            {
                Console.WriteLine("Diretório Base: {0}", Directory.GetCurrentDirectory());

                // Load Environment configuration
                var configuration = BuildConfiguration();

                // configure dependency injection
                var services = IoCBootstrap.Configure(configuration);

                config.SetName("WorkerOracleTemplate");
                config.SetDisplayName("WorkerOracleTemplate");
                config.SetDescription("Template para workers que conectam em Oracle");

                var name = config.GetDefaultName();

                var logger = services.GetService<ILogger<Program>>();

                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArgs, controller) =>
                    {
                        var microService = services.GetService<TesteService>();
                        return microService;
                    });

                    serviceConfig.OnStart((service, extraArguments) =>
                    {
                        logger.LogInformation("Serviço {0} iniciado com sucesso", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        logger.LogInformation("Serviço {0} parado com sucesso", name);
                        service.Stop();
                    });

                    serviceConfig.OnInstall(service =>
                    {
                        logger.LogInformation("Serviço {0} instalado com sucesso", name);
                    });

                    serviceConfig.OnUnInstall(service =>
                    {
                        logger.LogInformation("Serviço {0} desinstalado com sucesso", name);
                    });

                    serviceConfig.OnPause(service =>
                    {
                        logger.LogInformation("Serviço {0} parado com sucesso", name);
                    });

                    serviceConfig.OnContinue(service =>
                    {
                        logger.LogInformation("Serviço {0} reiniciado com sucesso", name);
                    });

                    serviceConfig.OnShutdown(service =>
                    {
                        logger.LogInformation("Serviço {0} desligado com sucesso", name);
                    });

                    serviceConfig.OnError(ex =>
                    {
                        logger.LogError(ex, "Erro ao executar o serviço {0}: {1}", name, ex.Message);
                    });
                });
            });
        }

        private static IConfiguration BuildConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration;
        }
    }
}
