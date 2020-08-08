using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Entidades.Audio
{
    public class AudioDeArquivo : AudioBase
    {
        public String Arquivo { get; }

        public AudioDeArquivo(string arquivo, string descricao):base(descricao)
        {
            Arquivo = arquivo;
        }

        protected override Stream ObterStream()
        {
            var ffmpeg_inf = new ProcessStartInfo
            {
                FileName = "Musicas/ffmpeg",
                Arguments = $"-i \"{Arquivo}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var ffmpeg = Process.Start(ffmpeg_inf);
            return ffmpeg.StandardOutput.BaseStream;
        }

        public override string ValidaAudio()
        {
            if (!File.Exists(Arquivo))
            {
                return $"O Arquivo \"{Arquivo}\" não existe.";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
