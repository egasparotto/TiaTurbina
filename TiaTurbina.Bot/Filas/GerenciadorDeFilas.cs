using DSharpPlus.CommandsNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Executores;

namespace TiaTurbina.Bot.Filas
{
    public class GerenciadorDeFilas
    {
        protected IDictionary<ulong, FilaDeReproducaoDeAudio> Filas { get; }

        public ExecutorDeAudio ExecutorDeAudio { get; }

        public GerenciadorDeFilas(ExecutorDeAudio executorDeAudio)
        {
            Filas = new Dictionary<ulong, FilaDeReproducaoDeAudio>();
            ExecutorDeAudio = executorDeAudio;
        }

        public FilaDeReproducaoDeAudio ObterFila(CommandContext ctx)
        {
            if (Filas.TryGetValue(ctx.Guild.Id, out FilaDeReproducaoDeAudio fila))
            {
                return fila;
            }
            else
            {
                var novaFila = new FilaDeReproducaoDeAudio(ExecutorDeAudio.ExecutarMusica);
                Filas.Add(ctx.Guild.Id, novaFila);
                return novaFila;
            }
        }
    }
}
