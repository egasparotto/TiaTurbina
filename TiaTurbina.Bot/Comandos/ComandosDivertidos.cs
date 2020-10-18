using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Bot.Comandos
{
    class ComandosDivertidos : BaseCommandModule
    {
        [Command("Ping")]
        [Description("Retorna Pong")]
        public async Task Ping(CommandContext contexto)
        {
            await contexto.Channel.SendMessageAsync("Pong");
        }

        [Command("Oi")]
        [Description("Um breve olá")]
        public async Task Oi(CommandContext contexto)
        {
            await contexto.Channel.SendMessageAsync($"Olá {contexto.Message.Author.Mention}");
            if (contexto.Guild != null)
            {
                await contexto.Channel.SendMessageAsync($"Espero que se divirta no {contexto.Guild.Name}");
            }
        }

        [Command("Aleatorio")]
        [Description("Retorna um número aleatorio")]
        public async Task Aleatorio(CommandContext contexto, [Description("Mínimo")] int minimo, [Description("Máximo")] int maximo)
        {
            await contexto.Channel.SendMessageAsync($"Seu número aleatório é: {new Random().Next(minimo, maximo)}");
        }

        [Command("Chefe")]
        [Description("Quem manda no servidor?")]
        public async Task Chefe(CommandContext contexto)
        {
            if (contexto.Guild?.Owner != null)
            {
                var chefe = contexto.Guild.Owner;
                var fotoDoChefe = chefe.GetAvatarUrl(DSharpPlus.ImageFormat.Auto);

                DiscordEmbed anexo = new DiscordEmbedBuilder()
                    .WithTitle("ATENÇÃO")
                    .WithDescription($"{chefe.Mention} manda no servidor")
                    .WithThumbnail(fotoDoChefe, 100, 100)
                    .WithColor(DiscordColor.IndianRed)
                    .Build();

                await contexto.Channel.SendMessageAsync(embed: anexo);
            }
        }
    }
}
