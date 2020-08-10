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
            {"Eunaotolouco", new AudioDeArquivo("Musicas/eunaotolouco.mp3", "Eu não to louco?") },
            {"Cavalo", new AudioDoYoutube(new Uri("https://www.youtube.com/watch?v=1xzGPPxKgJM"), "Cavalo") },
            {"Credimatone", new AudioDoYoutube(new Uri("https://www.youtube.com/watch?v=tz7E4nJMGvM"), "211-00-11") },
            {"HojeNao", new AudioDoYoutube(new Uri("https://www.youtube.com/watch?v=-Ihuae45HiY"), "Hoje sim, hoje sim") }
        };
    }
}
