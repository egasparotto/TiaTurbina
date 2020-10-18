using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TiaTurbina.Bot;

namespace TiaTurbina.Apresentacao.Web
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddLogging(log => log.AddConsole());
            services.IniciarBot();
        }

        public void Configure(IApplicationBuilder app, ExecutorDoBot executorDoBot, IConfiguration configuracao)
        {
#pragma warning disable CS4014 // Como esta chamada não é esperada, a execução do método atual continua antes de a chamada ser concluída
            executorDoBot.Executar();
#pragma warning restore CS4014 // Como esta chamada não é esperada, a execução do método atual continua antes de a chamada ser concluída

            app.Use(async (context, next) =>
            {
                await next();
                context.Response.Redirect($"https://discord.com/oauth2/authorize?client_id={configuracao.GetValue<string>("ID")}&scope=bot&permissions=1");
            });
        }
    }
}
