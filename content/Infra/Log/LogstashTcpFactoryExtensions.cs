using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorkerOracleTemplate.Infra.Log
{
    public static class LogstashTcpFactoryExtensions
    {
        public static ILoggerFactory AddLogtash(this ILoggerFactory loggerFactory, LogstashOptions options)
        {
            loggerFactory.AddProvider(new LogstashTcpLoggerProvider(options));
            return loggerFactory;
        }
    }
}