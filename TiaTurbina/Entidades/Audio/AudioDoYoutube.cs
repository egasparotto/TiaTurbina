using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Entidades.Youtube;

namespace TiaTurbina.Entidades.Audio
{
    public class AudioDoYoutube : AudioBase
    {
        public Uri URL { get; }
        public AudioDoYoutube(Uri url, string descricao) : base(descricao)
        {
            URL = url;
        }

        public override string ValidaAudio()
        {
            return "";
        }

        protected override async Task<Stream> ObterStream()
        {
            var video = ResolverLink();

            var processo = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "Musicas/ffmpeg",
                    Arguments = $"-xerror -i \"{(await video.ObterMelhorFormatoDeTransmissao()).Url}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true
                }
            };
            processo.Start();

            return processo.StandardOutput.BaseStream;
        }

        public Video ResolverLink()
        {
            var processo = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "Musicas/youtube-dl",
                    Arguments = $"-j --flat-playlist \"{URL}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            processo.Start();
            while (!processo.StandardOutput.EndOfStream)
            {
                return Video.DeJson(processo.StandardOutput.ReadLine());
            }
            return Video.DeJson(processo.StandardOutput.ReadLine());
        }
    }
}
