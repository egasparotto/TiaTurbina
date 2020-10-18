using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Bot.Entidades.Youtube
{
    public class FormatoDoVideo
    {
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Note { get; set; }
        public string Url { get; set; }
        public float? Bitrate { get; set; }
    }
}
