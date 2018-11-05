using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkerOracleTemplate.Repositories;

namespace WorkerOracleTemplate.Business
{
    public class TesteBusiness : IDisposable
    {
        private bool _disposed;
        private ILogger<TesteBusiness> _logger;
        private TesteRepository _testeRepository;

        public TesteBusiness(ILogger<TesteBusiness> logger, TesteRepository testeRepository)
        {
            _disposed = false;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testeRepository = testeRepository ?? throw new ArgumentNullException(nameof(testeRepository));
        }

        public async Task OnExecuteAsync()
        {
            var dbDateTime = await _testeRepository.GetCurrentDbDateTimeAsync();

            _logger.LogInformation("Data e hora atual do servidor: {0}", DateTimeOffset.Now.ToOffset(new TimeSpan(-3, 0, 0)).ToString("dd/MM/yyyy HH:mm:ss"));
            _logger.LogInformation("Data e hora atual do banco de dados: {0}", dbDateTime.CurrentTimestamp.ToOffset(new TimeSpan(-3 , 0, 0)).ToString("dd/MM/yyyy HH:mm:ss"));
        }

        public Task OnErrorAsync(Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar data e hora do sistema: {0}", ex.Message);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _logger = null;

            _disposed = true;
        }
    }
}