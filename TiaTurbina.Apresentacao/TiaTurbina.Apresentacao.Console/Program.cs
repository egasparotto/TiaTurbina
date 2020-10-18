using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Threading.Tasks;

using TiaTurbina.Bot;

namespace TiaTurbina.Apresentacao.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
           var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            IServiceCollection services = new ServiceCollection();


            services.AddSingleton<ServicoDeExecucaoDoBot>();
            services.AddSingleton<IConfiguration>(configuration);
            services.IniciarBot();

            var serviceProvider = services.BuildServiceProvider();
            var executor = serviceProvider.GetRequiredService<ServicoDeExecucaoDoBot>();

            await executor.Iniciar();

        }
    }
}
