using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AikidoWebsite.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseSentry()
                .UseKestrel(o => o.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();
            host.Run();
        }
    }
}
