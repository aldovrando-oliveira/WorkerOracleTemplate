using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WorkerOracleTemplate.Infra.Log
{
    public class LogstashTcpLogger : ILogger
    {
        private readonly LogstashTcpLoggerProvider _provider;
        private readonly string _categoryName;

        public LogstashTcpLogger(LogstashTcpLoggerProvider provider, string categoryName)
        {
            _provider = provider;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var builder = new StringBuilder();
            builder.AppendFormat("[{0}] ", eventId.ToString());
            builder.AppendFormat("{0}: ", _categoryName);

            if (state != null)
            {
                // TODO: implementar fluxo com escopo
            }

            builder.Append(formatter(state, exception));

            _provider.AddMessage(logLevel, builder.ToString(), exception);
        }
    }
}