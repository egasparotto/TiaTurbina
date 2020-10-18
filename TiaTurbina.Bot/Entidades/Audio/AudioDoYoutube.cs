using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Entidades.Youtube;

namespace TiaTurbina.Bot.Entidades.Audio
{
    public class AudioDoYoutube : AudioBase
    {
        public Uri URL { get; }
        public Video Video { get; }
        private MemoryStream Stream { get; }

        public AudioDoYoutube(Uri url, string descricao) : base(descricao)
        {
            URL = url;
            Video = ResolverLink();
            Stream = new MemoryStream();

            if (Video != null)
            {
                var melhorFormato = Video.ObterMelhorFormatoDeTransmissao().GetAwaiter().GetResult();

                var processo = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "Musicas/ffmpeg",
                        Arguments = $"-xerror -i \"{melhorFormato.Url}\" -ac 2 -f s16le -ar 48000 pipe:1",
                        RedirectStandardOutput = true
                    }
                };
                processo.Start();
                try
                {
                    processo.StandardOutput.BaseStream.CopyTo(Stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        public override string ValidaAudio()
        {
            if (Video == null || Stream == null)
            {
                return "Erro ao processar vídeo";
            }
            return "";
        }

        protected override Stream ObterStream()
        {
            Stream.Position = 0;
            return Stream;
        }

        private Video ResolverLink()
        {
            string caminho = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            string ydlPath = @"Python/python.exe";
            string ydl = caminho + @"/Python/youtube-dl";
            Process proc = new Process();

            try
            {
                proc.EnableRaisingEvents = false;
                proc.StartInfo.ErrorDialog = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.FileName = ydlPath;
                proc.StartInfo.Arguments = ydl + $" -j --flat-playlist \"{URL}\"";
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();

                var video = Video.DeJson(proc.StandardOutput.ReadLine());

                proc.WaitForExit();
                proc.Close();

                return video;
            }
            catch
            {
                return null;
            }
        }
    }
}
