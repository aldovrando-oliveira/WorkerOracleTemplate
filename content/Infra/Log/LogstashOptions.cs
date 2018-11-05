using System.Collections.Generic;

namespace WorkerOracleTemplate.Infra.Log
{
    public class LogstashOptions
    {
        public string AppName { get; set; }
        public string Host { get; set;  }
        public int Port { get; set; }
        public Dictionary<string, string> ExtraValues { get; set; }
    }
}