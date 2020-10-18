using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Filas;
using TiaTurbina.Bot.Listas;

namespace TiaTurbina.Bot.Comandos
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
            var gerenciadorDeFilas = (GerenciadorDeFilas)_serviceProvider.GetService(typeof(GerenciadorDeFilas));
            foreach (var audio in ListaDeAudios.Lista)
            {
                var comandoDinamico = new ComandosDinamicosDeAudio(audio.Value, gerenciadorDeFilas);
                Comando funcao = comandoDinamico.ExecutarMusica;
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
