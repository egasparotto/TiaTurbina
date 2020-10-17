using DSharpPlus;
using DSharpPlus.CommandsNext;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TiaTurbina.Comandos;
using TiaTurbina.Executores;
using TiaTurbina.Filas;

namespace TiaTurbina
{
    static class Inicializador
    {
        public static void IniciarBot(this IServiceCollection services)
        {
            services.AddSingleton<ExecutorDeAudio>();
            services.AddSingleton<GerenciadorDeFilas>();

            services.AddSingleton(x =>
            {
                var configuracao = x.GetRequiredService<IConfiguration>();
                return new DiscordConfiguration()
                {
                    Token = configuracao.GetValue<string>("Token"),
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
