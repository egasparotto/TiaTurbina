using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Entidades.Audio;
using TiaTurbina.Executores;
using TiaTurbina.Listas;

namespace TiaTurbina.Comandos
{
    public class Comandos
    {
        private readonly CommandsNextExtension _comandos;
        private readonly IServiceProvider _serviceProvider;

        public Comandos(DiscordClient cliente, CommandsNextConfiguration configuracao, IServiceProvider serviceProvider)
        {
            configuracao.Services = serviceProvider;
            _serviceProvider = serviceProvider;
            _comandos = cliente.UseCommandsNext(configuracao);
        }

        public delegate Task Comando(CommandContext ctx);

        public void IniciarComandos()
        {
            _comandos.RegisterCommands<ComandosDivertidos>();
            _comandos.RegisterCommands<ComandosDeAudio>();

            RegistrarMusicas();

        }

        private void RegistrarMusicas()
        {
            foreach (var audio in ListaDeAudios.Lista)
            {
                var executor = new ExecutorDeAudio(audio.Value);
                Comando funcao = executor.ExecutarMusica;
                var funcaoDoComando = new CommandOverloadBuilder(funcao);
                var argumentos = funcaoDoComando.Arguments;
                var comando = new CommandBuilder()
                                  .WithName(audio.Key)
                                  .WithOverload(funcaoDoComando)
                                  .WithDescription(audio.Value.Descricao);
                _comandos.RegisterCommands(comando);
            };
        }
    }
}
