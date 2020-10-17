using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Entidades.Audio;
using TiaTurbina.Filas;

namespace TiaTurbina.Comandos
{
    class ComandosDeAudio : BaseCommandModule
    {
        public GerenciadorDeFilas GerenciadorDeFilas { get; }

        public ComandosDeAudio(GerenciadorDeFilas gerenciadorDeFilas)
        {
            GerenciadorDeFilas = gerenciadorDeFilas;
        }

        [Command("Parar")]
        [Description("Para a execução de uma música")]
        public async Task Parar(CommandContext ctx)
        {
            try
            {
                var vnext = ctx.Client.GetVoiceNext();

                var vnc = vnext.GetConnection(ctx.Guild);
                if (vnc == null)
                    ctx.Client.Logger.LogInformation("Tia Turbina", "Bot não está conectado neste canal", DateTime.Now);

                vnc.Disconnect();
                GerenciadorDeFilas.ObterFila(ctx).LimparFila();
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
            }
        }

        [Command("P")]
        [Description("Executar um audio a partir de um vídeo do youtube")]
        public async Task ExecutarVideo(CommandContext ctx, [Description("URL do vídeo")] string URL)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":arrows_counterclockwise:"));
            await GerenciadorDeFilas.ObterFila(ctx).Adicionar(new ExecucaoDeAudio(new AudioDoYoutube(new Uri(URL),"Audio em tempo de Execucao"), ctx));
        }
    }
}
