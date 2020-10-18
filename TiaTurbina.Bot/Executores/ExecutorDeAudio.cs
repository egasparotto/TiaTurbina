using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Entidades.Audio;

namespace TiaTurbina.Bot.Executores
{
    public class ExecutorDeAudio
    {
        public delegate Task FinalizarExecucao(CommandContext commandContext, VoiceNextConnection conexaoDeVoz);

        public FinalizarExecucao AoFinalizarExecucao { get; set; }

        private async Task<VoiceNextConnection> Entrar(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
                ctx.Client.Logger.LogInformation(new EventId(1, "Tia Turbina"), "Bot ja conectado neste canal");

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                ctx.Client.Logger.LogInformation(new EventId(1, "Tia Turbina"), "Usuário deve estar conectado em um canal de audio");
                return null;
            }

            return await vnext.ConnectAsync(chn);
        }

        internal async Task ExecutarMusica(ExecucaoDeAudio execucaoDeAudio)
        {
            var ctx = execucaoDeAudio.CommandContext;
            var audio = execucaoDeAudio.Audio;
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                // not enabled
                ctx.Client.Logger.LogError(new EventId(1, "Tia Turbina"), "VNext não Habilitado");
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                vnc = await Entrar(ctx);
                if (vnc == null)
                {
                    return;
                }
            }

            var validacao = audio.ValidaAudio();
            // check if file exists
            if (!string.IsNullOrEmpty(validacao))
            {
                // file does not exist
                ctx.Client.Logger.LogError(new EventId(1, "Tia Turbina"), validacao);
                return;
            }

            // wait for current playback to finish
            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            // play
            await vnc.SendSpeakingAsync(true);
            try
            {
                await audio.TransmitirAudio(vnc);
            }
            finally
            {
                while (vnc.IsPlaying)
                    await vnc.WaitForPlaybackFinishAsync();
                await AoFinalizarExecucao?.Invoke(ctx, vnc);
            }
        }
    }
}
