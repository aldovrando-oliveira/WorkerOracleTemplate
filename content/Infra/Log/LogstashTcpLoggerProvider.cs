using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetStash.Log;

namespace WorkerOracleTemplate.Infra.Log
{
    [ProviderAlias("Logstash")]
    public class LogstashTcpLoggerProvider : ILoggerProvider
    {
        private bool _disposed;
        private readonly NetStashLog _netStash;
        private readonly Dictionary<string, string> _extraValues;

        public LogstashTcpLoggerProvider(IOptions<LogstashOptions> options) : this(options.Value)
        {
        }

        public LogstashTcpLoggerProvider(LogstashOptions options)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            _disposed = false;
            _netStash = new NetStashLog(options.Host, options.Port, appName, options.AppName ?? appName);
            _extraValues = options.ExtraValues ?? new Dictionary<string, string>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LogstashTcpLogger(this, categoryName);
        }

        public void AddMessage(LogLevel level, string message, Exception exception, Dictionary<string, string> addtionalValues)
        {
            if (addtionalValues != null)
                foreach(var item in addtionalValues)
                    _extraValues.Add(item.Key, item.Value);

            switch (level)
            {
                case LogLevel.Trace:
                    _netStash.Verbose(message, _extraValues);
                    break;
                case LogLevel.Debug:
                    _netStash.Debug(message, _extraValues);
                    break;
                case LogLevel.Information:
                    _netStash.Information(message, _extraValues);
                    break;
                case LogLevel.Warning:
                    _netStash.Warning(message, _extraValues);
                    break;
                case LogLevel.Error:
                    if (exception == null)
                        _netStash.Error(message, _extraValues);
                    else
                    {
                        var newExtra = new Dictionary<string, string>(_extraValues);
                        newExtra.Add("message", message);
                        _netStash.Error(exception, newExtra);
                    }
                    break;
                case LogLevel.Critical:
                    _netStash.Fatal(message, _extraValues);
                    break;
            }
        }

        public void AddMessage(LogLevel level, string message, Exception exception)
        {
            AddMessage(level, message, exception, null);
        }

        public void AddMessage(LogLevel level, string message)
        {
            AddMessage(level, message, null);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _netStash.Stop();
            _extraValues.Clear();

            _disposed = true;
        }
    }
}