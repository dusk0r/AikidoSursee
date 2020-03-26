using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AikidoWebsite.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

            var host = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseSentry()
                .UseKestrel(o => o.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .Build();
            host.Run();
        }
    }
}
