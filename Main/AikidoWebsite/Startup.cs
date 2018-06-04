using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;

namespace AikidoWebsite.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json")
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRavenDB(Configuration["dbURL"], Configuration["dbName"]);

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }

    }

    public static class AddStartupExtensions
    {
        public static void AddRavenDB(this IServiceCollection services, string url, string database, string certificate = null)
        {
            var urls = new string[] { url };

            var ravendb = new DocumentStore {
                Urls = urls,
                Database = database,
                Certificate = certificate != null ? new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(certificate)) : null
            };
            ravendb.Initialize();
        }
    }
}
