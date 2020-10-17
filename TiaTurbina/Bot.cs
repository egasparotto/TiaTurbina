using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;

using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TiaTurbina.Comandos;

namespace TiaTurbina
{

    class Bot: IHostedService
    {
        public Bot(DiscordClient cliente, Comandos.Comandos comandos)
        {
            Cliente = cliente;
            Comandos = comandos;
        }

        public DiscordClient Cliente { get; }
        public Comandos.Comandos Comandos { get; }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Cliente.UseVoiceNext();

            Cliente.Ready += Cliente_Ready;
            Cliente.ClientErrored += Cliente_ClientErrored;


            Comandos.IniciarComandos();

            await Cliente.ConnectAsync();

            await Task.Delay(-1);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task Cliente_ClientErrored(DiscordClient client, ClientErrorEventArgs e)
        {
            var atividade = new DiscordActivity("Vish Pifei", ActivityType.Streaming) 
            { 
                StreamUrl = "https://www.twitch.tv/avj255"
            };
            await Cliente.UpdateStatusAsync(activity: atividade, userStatus: UserStatus.Idle);
        }

        private async Task Cliente_Ready(DiscordClient cliente, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            var atividade = new DiscordActivity("Bot do MetaGames", ActivityType.Streaming)
            {
                StreamUrl = "https://www.twitch.tv/avj255"
            };
            await Cliente.UpdateStatusAsync(activity: atividade, userStatus: UserStatus.Online);
        }
    }
}
