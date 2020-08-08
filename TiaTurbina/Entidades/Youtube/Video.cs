using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Entidades.Youtube
{
    public class Video
    {
        private FormatoDoVideo melhorFormatoDeTransmissão;

        public string Titulo { get; }
        public string Id { get; }
        private Task<IEnumerable<FormatoDoVideo>> Formatos { get; set; }

        public Video(string id, string titulo = null)
        {
            Id = id;
            Titulo = titulo;
        }

        public async Task<FormatoDoVideo> ObterMelhorFormatoDeTransmissao()
        {
            if (melhorFormatoDeTransmissão != null) return melhorFormatoDeTransmissão;
            var videoFormatos = await ObterFormatos();
            if (videoFormatos == null) return null;
            var formatosDeAudio = videoFormatos.Where(x => x.Extension == "m4a");
            if (formatosDeAudio.Any())
            {
                melhorFormatoDeTransmissão = formatosDeAudio.OrderByDescending(x => x.Bitrate).First();
                return melhorFormatoDeTransmissão;
            }

            melhorFormatoDeTransmissão = videoFormatos.Where(x => x.Extension == "webm" || x.Extension == "mp4")
                .Where(x => x.Width != null && x.Height != null || x.Bitrate != null)
                .OrderByDescending(x => x.Bitrate ?? x.Width * x.Height)
                .First();

            return melhorFormatoDeTransmissão;
        }

        public static Video DeJson(string json)
        {
            var jo = JObject.Parse(json);
            var vid = new Video(jo["id"].ToString(), jo["title"].ToString())
            {
                Formatos = Task.FromResult(ResolveFormatos(jo["formats"].ToString()))
            };
            return vid;
        }

        private static IEnumerable<FormatoDoVideo> ResolveFormatos(string jsonArray)
        {
            var ja = JArray.Parse(jsonArray);
            return ja
                .Select(x =>
                    new FormatoDoVideo
                    {
                        FormatCode = x["format_id"].ToString(),
                        Extension = x["ext"].ToString(),
                        Width = x.Value<int?>("width"),
                        Height = x.Value<int?>("height"),
                        Note = x["format_note"].ToString(),
                        Bitrate = x.Value<float?>("abr") ?? x.Value<float?>("tbr"),
                        Url = x["url"].ToString()
                    }
                );
        }

        public Task<IEnumerable<FormatoDoVideo>> ObterFormatos()
        {
            if (Formatos != null) return Formatos;
            var t = Task.Factory.StartNew(() => {
                var processo = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "Musicas/youtube-dl",
                        Arguments = "-j " + Id,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                processo.Start();
                var saidaNormal = processo.StandardOutput.ReadToEnd();
                var saidaErro = processo.StandardError.ReadToEnd();
                processo.WaitForExit();
                var codigoDeErro = processo.ExitCode;
                if (codigoDeErro != 0)
                {
                    return null;
                }
                var jo = JObject.Parse(saidaNormal);
                return ResolveFormatos(jo["formats"].ToString());
            });
            Formatos = t;
            return t;
        }
    }
}
