using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Entidades.Audio;
using TiaTurbina.Bot.Filas;

namespace TiaTurbina.Bot.Comandos
{
    public class ComandosDinamicosDeAudio
    {
        public AudioBase Audio { get; }
        public GerenciadorDeFilas GerenciadorDeFilas { get; }

        public ComandosDinamicosDeAudio(AudioBase audio, GerenciadorDeFilas gerenciadorDeFilas)
        {
            Audio = audio;
            GerenciadorDeFilas = gerenciadorDeFilas;
            GerenciadorDeFilas.ExecutorDeAudio.AoFinalizarExecucao = FinalizarExecucao;
        }

        internal async Task ExecutarMusica(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":arrows_counterclockwise:"));
            await GerenciadorDeFilas.ObterFila(ctx).Adicionar(new ExecucaoDeAudio(Audio, ctx));
        }

        private async Task FinalizarExecucao(CommandContext ctx, VoiceNextConnection vnc)
        {
            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":arrows_counterclockwise:"));
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            await GerenciadorDeFilas.ObterFila(ctx).Remover(ctx, vnc);
        }
    }
}
