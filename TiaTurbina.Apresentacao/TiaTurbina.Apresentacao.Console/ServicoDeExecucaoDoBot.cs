using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using TiaTurbina.Bot;

namespace TiaTurbina.Apresentacao.Console
{
    class ServicoDeExecucaoDoBot
    {
        private ExecutorDoBot ExecutorDoBot { get; }

        public ServicoDeExecucaoDoBot(ExecutorDoBot executorDoBot)
        {
            ExecutorDoBot = executorDoBot;
        }

        public async Task Iniciar()
        {
            await ExecutorDoBot.Executar();
        }
    }
}
