using System;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using PaulMiami.AspNetCore.Mvc.Recaptcha;

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
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRavenDB(Configuration["dbURL"], Configuration["dbName"]);

            services.AddIdentity<Benutzer, Role>()
                .AddUserStore<RavenUserProvider>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"]
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHttpsRedirection();
                //app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }

    }

    public static class AddStartupExtensions
    {
        public static void AddRavenDB(this IServiceCollection services, string url, string database, string certificateString = null)
        {
            var urls = new string[] { url };
            var certificate = certificateString != null ? new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(certificateString)) : null;

            var documentStore = new DocumentStore {
                Urls = urls,
                Database = database,
                Certificate = certificate
            };
            documentStore.Initialize();

            services.AddScoped<IDocumentSession>(isp => documentStore.OpenSession());
            services.AddScoped<IAsyncDocumentSession>(isp => documentStore.OpenAsyncSession());
            services.AddSingleton<IDocumentStore>(documentStore);
        }
    }
}
