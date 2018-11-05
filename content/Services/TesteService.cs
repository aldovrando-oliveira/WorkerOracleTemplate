using System;
using Microsoft.Extensions.Options;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using WorkerOracleTemplate.Business;
using WorkerOracleTemplate.Models.Options;

namespace WorkerOracleTemplate.Services
{
    public class TesteService : MicroService, IMicroService
    {
        private WorkerOptions _options;
        private readonly TesteBusiness _business;

        public TesteService(IOptions<WorkerOptions> options, TesteBusiness business)
        {
            _options = options.Value;
            _business = business ?? throw new ArgumentNullException(nameof(business), "Argumento {0} é obrigatório");
        }

        public void Start()
        {
            StartBase();
            Timers.Start("TesteWorker", _options.DelayExecutionInMiliSeconds, () => _business.OnExecuteAsync().Wait(), (ex) => _business.OnErrorAsync(ex).Wait());
        }

        public void Stop()
        {
            StopBase();
        }
    }
}