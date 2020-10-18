using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Entidades.Audio;

namespace TiaTurbina.Bot.Filas
{
    public class FilaDeReproducaoDeAudio
    {
        public delegate Task ExecutarMusica(ExecucaoDeAudio execucaoDeAudio);


        protected Queue<ExecucaoDeAudio> Fila { get; }
        protected ExecutarMusica AoExecutarMusica { get; }
        public EnumeradorDeStatusDaFila StatusDaFila { get; private set; }

        public FilaDeReproducaoDeAudio(ExecutarMusica aoExecutarMusica)
        {
            Fila = new Queue<ExecucaoDeAudio>();
            StatusDaFila = EnumeradorDeStatusDaFila.Parada;
            AoExecutarMusica = aoExecutarMusica;
        }

        public async Task Adicionar(ExecucaoDeAudio execucaoDeAudio)
        {
            if (StatusDaFila == EnumeradorDeStatusDaFila.Parada)
            {
                StatusDaFila = EnumeradorDeStatusDaFila.Executando;
                try
                {
                    await AoExecutarMusica?.Invoke(execucaoDeAudio);
                }
                catch
                {
                    await execucaoDeAudio.CommandContext.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(execucaoDeAudio.CommandContext.Client, ":arrows_counterclockwise:"));
                    await execucaoDeAudio.CommandContext.Message.CreateReactionAsync(DiscordEmoji.FromName(execucaoDeAudio.CommandContext.Client, ":thumbsdown:"));
                    Fila.Clear();
                    StatusDaFila = EnumeradorDeStatusDaFila.Parada;
                    await execucaoDeAudio.CommandContext.Message.RespondAsync(content: "Erro ao executar o comando, a fila de audios foi limpa");
                }
            }
            else
            {
                Fila.Enqueue(execucaoDeAudio);
            }
        }

        public void LimparFila()
        {
            Fila.Clear();
            StatusDaFila = EnumeradorDeStatusDaFila.Parada;
        }

        public async Task Remover(CommandContext ctx, VoiceNextConnection vnc)
        {
            if (Fila.Count == 0)
            {
                await vnc.SendSpeakingAsync(false);
                vnc?.Disconnect();
                StatusDaFila = EnumeradorDeStatusDaFila.Parada;
            }
            else
            {
                var proximaExecucao = Fila.Dequeue();
                await AoExecutarMusica?.Invoke(proximaExecucao);
            }
        }
    }
}
