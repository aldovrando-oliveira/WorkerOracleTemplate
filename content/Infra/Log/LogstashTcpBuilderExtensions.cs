using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorkerOracleTemplate.Infra.Log
{
    public static class LogstashTcpBuilderExtensions
    {
        public static ILoggingBuilder AddLogstash(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, LogstashTcpLoggerProvider>();
            return builder;
        }

        public static ILoggingBuilder AddLogstash(this ILoggingBuilder builder, Action<LogstashOptions> configure)
        {
            builder.AddLogstash();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}