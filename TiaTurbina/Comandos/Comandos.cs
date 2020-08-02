using DSharpPlus;
using DSharpPlus.CommandsNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Comandos
{
    public class Comandos
    {
        private readonly CommandsNextExtension _comandos;

        public Comandos(DiscordClient cliente, CommandsNextConfiguration configuracao, IServiceProvider serviceProvider)
        {
            configuracao.Services = serviceProvider;
            _comandos = cliente.UseCommandsNext(configuracao);
        }

        public void IniciarComandos()
        {
            _comandos.RegisterCommands<ComandosDivertidos>();
            _comandos.RegisterCommands<ComandosDeAudio>();
        }
    }
}
