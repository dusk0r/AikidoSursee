using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Web.Security;
using AikidoWebsite.Web.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using AikidoWebsite.Data.Listener;
using Microsoft.Extensions.Logging;
using AikidoWebsite.Data.Migration;
using Microsoft.Extensions.Hosting;

namespace AikidoWebsite.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
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
            var documentStore = services.AddRavenDB(Configuration["DB:Url"], Configuration["DB:Name"], Configuration["DB_Certificate"]);

            //services.AddIdentity<Benutzer, Role>()
            //.AddRoleManager
            //.AddUserStore<RavenUserProvider>()
            //.AddRoleStore<RavenRoleStore>();
            //services.AddIdentityCore<Benutzer>()
            //    .AddUserStore<RavenUserProvider>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {

                });
                //.AddGoogle(options =>
                //{
                //    options.ClientId = Configuration["Authentication_Google_ClientId"];
                //    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

                //    var baseOnCreatingTicket = options.Events.OnCreatingTicket;
                //    options.Events.OnCreatingTicket = async ctx =>
                //    {
                //        await baseOnCreatingTicket(ctx);
                //        var id = ctx.User["id"].ToString();

                //        using (var documentSession = documentStore.OpenSession())
                //        {
                //            var benutzer = documentSession.Query<Benutzer>().FirstOrDefault(b => b.GoogleLogin == id && b.IstAktiv);

                //            if (benutzer != null)
                //            {
                //                foreach (var claim in ctx.Identity.Claims.ToList())
                //                {
                //                    ctx.Identity.RemoveClaim(claim);
                //                }
                //                var claims = new List<Claim> {
                //                    new Claim(ClaimTypes.NameIdentifier, benutzer.EMail),
                //                    new Claim(ClaimTypes.Name, benutzer.Name),
                //                    new Claim(ClaimTypes.Email, benutzer.EMail)
                //                };
                //                foreach (var role in benutzer.Gruppen)
                //                {
                //                    claims.Add(new Claim(ClaimTypes.Role, role));
                //                }
                //                ctx.Identity.AddClaims(claims);
                //            }
                //        }
                //    };
                //})
                //.AddTwitter(options =>
                //{
                //    options.ConsumerKey = Configuration["Authentication_Twitter_ConsumerKey"];
                //    options.ConsumerSecret = Configuration["Authentication_Twitter_ConsumerSecret"];

                //    var baseOnCreatingTicket = options.Events.OnCreatingTicket;
                //    options.Events.OnCreatingTicket = async ctx =>
                //    {
                //        await baseOnCreatingTicket(ctx);
                //        var id = ctx.UserId;

                //        using (var documentSession = documentStore.OpenSession()) {
                //            var benutzer = documentSession.Query<Benutzer>().FirstOrDefault(b => b.TwitterLogin == id && b.IstAktiv);

                //            if (benutzer != null) {
                //                if (ctx.Principal != null) {
                //                    //foreach (var claim in ctx.Principal.Claims.ToList()) {
                //                    //    ctx.Principal. .Claims.RemoveClaim(claim);
                //                    //}
                //                }

                //                //var claims = new List<Claim> {
                //                //    new Claim(ClaimTypes.NameIdentifier, benutzer.EMail),
                //                //    new Claim(ClaimTypes.Name, benutzer.Name),
                //                //    new Claim(ClaimTypes.Email, benutzer.EMail)
                //                //};
                //                //foreach (var role in benutzer.Gruppen) {
                //                //    claims.Add(new Claim(ClaimTypes.Role, role));
                //                //}
                //                //ctx.Identity.AddClaims(claims);
                //            }
                //        }
                //    };
                //});

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha_SiteKey"],
                SecretKey = Configuration["Recaptcha_SecretKey"],
                LanguageCode = "de"
            });

            services.AddClock();
            services.AddFlickr(Configuration["Flickr_ApiKey"]);
            services.AddTwitter(Configuration["Twitter_ConsumerKey"], Configuration["Twitter_ConsumerSecret"], Configuration["Twitter_AccessToken"], Configuration["Twitter_AccessSecret"]);
            services.AddBackupToken(Configuration["Backup_Secret"]);

            services.AddAntiforgery();
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute("faq", "/faq", defaults: new { controller = "Content", action = "Show", id = "faq" });
                routes.MapRoute("glossar", "/glossar", defaults: new { controller = "Content", action = "Show", id = "glossar" });
                routes.MapRoute("techniken", "/techniken", defaults: new { controller = "Content", action = "Show", id = "techniken" });
                routes.MapRoute("presse", "/presse", defaults: new { controller = "Content", action = "Show", id = "presse" });
                routes.MapRoute("links", "/links", defaults: new { controller = "Content", action = "Show", id = "links" });
                routes.MapRoute("trainingszeiten", "/trainingszeiten", defaults: new { controller = "Content", action = "Show", id = "trainingszeiten" });
                routes.MapRoute("einfuehrungskurse", "/einfuehrungskurse", defaults: new { controller = "Content", action = "Show", id = "einfuehrungskurse" });
                routes.MapRoute("bilder", "/bilder", defaults: new { controller = "Dojo", action = "Bilder" });
                routes.MapRoute("personen", "/personen", defaults: new { controller = "Dojo", action = "Personen" });
                routes.MapRoute("standort", "/standort", defaults: new { controller = "Dojo", action = "Standort" });
                routes.MapRoute("velos", "/velos", defaults: new { controller = "Content", action = "Show", id = "velos" });
                routes.MapRoute("kontakt", "/kontakt", defaults: new { controller = "Dojo", action = "Kontakt" });
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }

    public static class AddStartupExtensions
    {
        public static IDocumentStore AddRavenDB(this IServiceCollection services, string url, string database, string certificateString = null)
        {
            var urls = new string[] { url };
            var certificate = !String.IsNullOrEmpty(certificateString) ? new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(certificateString)) : null;

            var documentStore = new DocumentStore {
                Urls = urls,
                Database = database,
                Certificate = certificate
            };
            documentStore.Initialize();

            // Index
            IndexCreation.CreateIndexes(Assembly.GetAssembly(typeof(IEntity)), documentStore);

            // Listener
            documentStore.OnBeforeStore += SeiteStoreListener.BeforeStore;

            services.AddScoped<IDocumentSession>(isp => documentStore.OpenSession());
            services.AddScoped<IAsyncDocumentSession>(isp => documentStore.OpenAsyncSession());
            services.AddSingleton<IDocumentStore>(documentStore);

            // Migration
            Migration.DoMigration(documentStore);

            return documentStore;
        }

        public static void AddFlickr(this IServiceCollection services, string flickrApiKey)
        {
            services.AddSingleton(new FlickrService(flickrApiKey));
        }

        public static void AddClock(this IServiceCollection services)
        {
            services.AddSingleton<IClock>(new Clock());
        }

        public static void AddTwitter(this IServiceCollection services, string consumerKey, string consumerSecret, string userAccessToken, string userAccessSecret)
        {
            services.AddSingleton(new TwitterService(consumerKey, consumerSecret, userAccessToken, userAccessSecret));
        }

        public static void AddBackupToken(this IServiceCollection services, string secret)
        {
            services.AddSingleton(new BackupTokenService(secret, new Clock()));
        }
    }
}
