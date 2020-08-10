using DSharpPlus.CommandsNext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina.Entidades.Audio
{
    public class ExecucaoDeAudio
    {
        public ExecucaoDeAudio(AudioBase audio, CommandContext commandContext)
        {
            Audio = audio;
            CommandContext = commandContext;
        }

        public AudioBase Audio { get; }
        public CommandContext CommandContext { get; }
    }
}
