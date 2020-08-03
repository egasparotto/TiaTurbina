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

namespace TiaTurbina.Comandos
{
    class ComandosDeAudio : BaseCommandModule
    {
        private async Task<VoiceNextConnection> Entrar(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "Tia Turbina", "Bot ja conectado neste canal", DateTime.Now);

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina", "Usuário deve estar conectado em um canal de audio", DateTime.Now);

            return await vnext.ConnectAsync(chn);
        }

        private async Task ExecutarMusica(CommandContext ctx, string filename)
        {
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                // not enabled
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina","VNext não Habilitado", DateTime.Now);
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                vnc = await Entrar(ctx);
            }

            // check if file exists
            if (!File.Exists(filename))
            {
                // file does not exist
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error,"Tia Turbina",$"O Arquivo \"{filename}\" não existe.",DateTime.Now);
                return;
            }

            // wait for current playback to finish
            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            // play
            await vnc.SendSpeakingAsync(true);
            try
            {
                var ffmpeg_inf = new ProcessStartInfo
                {
                    FileName = "Musicas/ffmpeg",
                    Arguments = $"-i \"{filename}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var ffmpeg = Process.Start(ffmpeg_inf);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                // let's buffer ffmpeg output
                using (var ms = new MemoryStream())
                {
                    await ffout.CopyToAsync(ms);
                    ms.Position = 0;

                    var buff = new byte[3840]; // buffer to hold the PCM data
                    var br = 0;
                    while ((br = ms.Read(buff, 0, buff.Length)) > 0)
                    {
                        if (br < buff.Length) // it's possible we got less than expected, let's null the remaining part of the buffer
                            for (var i = br; i < buff.Length; i++)
                                buff[i] = 0;

                        await vnc.GetTransmitStream(20).WriteAsync(buff, 0, br); // we're sending 20ms of data
                    }
                }
            }
            finally
            {
                await vnc.SendSpeakingAsync(false);
            }
        }

        private async Task SairAposTerminarMusica(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                // not enabled
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina", "VNext não Habilitado", DateTime.Now);
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            while (vnc != null && vnc.IsPlaying)
            {
                await Task.Delay(30000);
            }
            vnc?.Disconnect();
        }

        [Command("Vitoria")]
        [Description("Executa o tema da vitória")]
        private async Task Vitoria(CommandContext ctx)
        {
            try
            {
                await ExecutarMusica(ctx, "Musicas/vitoria.m4a");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                await SairAposTerminarMusica(ctx);
            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
            }
        }

        [Command("Finivest")]
        [Description("Executa o tema do finivest")]
        private async Task Finivest(CommandContext ctx)
        {
            try
            {
                await ExecutarMusica(ctx, "Musicas/finivest.m4a");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                await SairAposTerminarMusica(ctx);
            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
            }
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
                    ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "Tia Turbina", "Bot não está conectado neste canal", DateTime.Now);

                vnc.Disconnect();
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            }
            catch
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
            }
        }
    }
}
