using DSharpPlus;
using DSharpPlus.CommandsNext;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Bot.Comandos;
using TiaTurbina.Bot.Executores;
using TiaTurbina.Bot.Filas;

namespace TiaTurbina.Bot
{
    public static class Inicializador
    {
        public static void IniciarBot(this IServiceCollection services)
        {
            services.AddSingleton<ExecutorDoBot>();
            services.AddSingleton<ExecutorDeAudio>();
            services.AddSingleton<GerenciadorDeFilas>();

            services.AddSingleton(x =>
            {
                var configuracao = x.GetRequiredService<IConfiguration>();
                return new DiscordConfiguration()
                {
                    Token = configuracao.GetSection("Token").Value,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true

                };
            });

            services.AddSingleton<DiscordClient>();

            services.AddSingleton(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "?", "/t ", "\\t " },
            });

            services.AddSingleton<Comandos.Comandos>();
        }
    }
}
