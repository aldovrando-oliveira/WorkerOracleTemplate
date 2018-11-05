namespace WorkerOracleTemplate.Infra.Database
{
    public class ConnectionOptions
    {
        public string DataSource { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ConnectionPoolingOptions Pooling { get; set; }

        public class ConnectionPoolingOptions 
        {
            public bool Pooling { get; set; } = false;
            public int MinSize { get; set; } = 3;
            public int MaxSize { get; set; } = 5;
        }
    }
}