using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Entidades.Audio;

namespace TiaTurbina.Listas
{
    public static class ListaDeAudios
    {
        public static IDictionary<string, AudioBase> Lista => new Dictionary<string, AudioBase>()
        {
            {"Vitoria", new AudioDeArquivo("Musicas/vitoria.m4a", "Tema da Vítoria") },
            {"Finivest", new AudioDeArquivo("Musicas/finivest.m4a", "Tema do Finivest") },
            {"Eunaotolouco", new AudioDeArquivo("Musicas/eunaotolouco.mp3", "Eu não to louco?") }
        };
    }
}
