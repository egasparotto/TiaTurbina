using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TiaTurbina.Bot;

namespace TiaTurbina.Apresentacao.Web
{
    public class ServicoDeExecucaoDoBot : IHostedService
    {
        ExecutorDoBot ExecutorDoBot { get; }

        public ServicoDeExecucaoDoBot(ExecutorDoBot executorDoBot)
        {
            ExecutorDoBot = executorDoBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ExecutorDoBot.Executar();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
