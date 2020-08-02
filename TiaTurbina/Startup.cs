using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiaTurbina
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(log => log.AddConsole());
            services.IniciarBot();
        }

        public void Configure(IApplicationBuilder app, Bot bot)
        {
            bot.ExecutarAsync();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Bot sendo Executado");
            });
        }
    }
}
