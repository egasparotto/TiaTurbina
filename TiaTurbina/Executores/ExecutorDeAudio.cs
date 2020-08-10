using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Entidades.Audio;
using TiaTurbina.Filas;

namespace TiaTurbina.Executores
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
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "Tia Turbina", "Bot ja conectado neste canal", DateTime.Now);

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina", "Usuário deve estar conectado em um canal de audio", DateTime.Now);

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
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina", "VNext não Habilitado", DateTime.Now);
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                vnc = await Entrar(ctx);
            }

            var validacao = audio.ValidaAudio();
            // check if file exists
            if (!string.IsNullOrEmpty(validacao))
            {
                // file does not exist
                ctx.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "Tia Turbina", validacao, DateTime.Now);
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
