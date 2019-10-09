using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Hallowspeak
{
    public class Program
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync(_cancellationTokenSource.Token);
        }

        public static void StopServer() => _cancellationTokenSource.Cancel();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://0.0.0.0:5050");
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
    }
}
