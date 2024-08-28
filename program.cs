using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ServiceBusWorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<IServiceBusService>(provider => new ServiceBusService(
                        provider.GetRequiredService<ILogger<ServiceBusService>>(),
                        context.Configuration["ServiceBus:ConnectionString"],
                        context.Configuration["ServiceBus:QueueName"]));

                    services.AddHostedService<Worker>();
                });
    }
}
