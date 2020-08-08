using DSharpPlus.VoiceNext;

using System.IO;
using System.Threading.Tasks;

namespace TiaTurbina.Entidades.Audio
{
    public abstract class AudioBase
    {
        protected abstract Task<Stream> ObterStream();

        public abstract string ValidaAudio();

        public string Descricao { get; }

        protected AudioBase(string descricao)
        {
            Descricao = descricao;
        }

        public async Task TransmitirAudio(VoiceNextConnection vnc)
        {
            using (var ms = new MemoryStream())
            {
                var stream = await ObterStream();

                await stream.CopyToAsync(ms);
                ms.Position = 0;

                var buff = new byte[3840]; // buffer to hold the PCM data
                var br = 0;
                while ((br = ms.Read(buff, 0, buff.Length)) > 0)
                {
                    if (br < buff.Length) // it's possible we got less than expected, let's null the remaining part of the buffer
                        for (var i = br; i < buff.Length; i++)
                            buff[i] = 0;

                    await vnc.GetTransmitStream(20).WriteAsync(buff, 0, br); // we're sending 20ms of data
                }
            }
        }
    }
}
