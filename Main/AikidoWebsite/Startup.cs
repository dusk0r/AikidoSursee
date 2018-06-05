using System;
using System.Reflection;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Web.Security;
using AikidoWebsite.Web.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

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

            //services.AddIdentity<Benutzer, Role>()
            //    .AddUserStore<RavenUserProvider>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {

                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"],
                LanguageCode = "de"
            });

            services.AddClock();
            services.AddFlickr(Configuration["Flickr:ApiKey"]);

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

            IndexCreation.CreateIndexes(Assembly.GetAssembly(typeof(IEntity)), documentStore);

            services.AddScoped<IDocumentSession>(isp => documentStore.OpenSession());
            services.AddScoped<IAsyncDocumentSession>(isp => documentStore.OpenAsyncSession());
            services.AddSingleton<IDocumentStore>(documentStore);
        }

        public static void AddFlickr(this IServiceCollection services, string flickrApiKey)
        {
            services.AddSingleton(new FlickrService(flickrApiKey));
        }

        public static void AddClock(this IServiceCollection services)
        {
            services.AddSingleton<IClock>(new Clock());
        }
    }
}
