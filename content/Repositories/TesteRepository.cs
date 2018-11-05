using System;
using System.Linq;
using System.Threading.Tasks;
using WorkerOracleTemplate.Infra.Database;
using WorkerOracleTemplate.Models.Teste;

namespace WorkerOracleTemplate.Repositories
{
    public class TesteRepository : IDisposable
    {
        private bool _disposed;
        private readonly Connection _connection;

        public TesteRepository(Connection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<TesteModel> GetCurrentDbDateTimeAsync()
        {
            await _connection.OpenConnectionAsync();

            var result = await _connection.ExecQueryAsync<TesteModel>("SELECT SYSDATE CurrentTimestamp FROM DUAL");

            return result.FirstOrDefault();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _connection.Dispose();

            _disposed = true;
        }
    }
}