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
using Rollbar;
using Rollbar.AspNetCore;
using Microsoft.Extensions.Logging;
using AikidoWebsite.Data.Migration;

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
            var documentStore = services.AddRavenDB(Configuration["DB:Url"], Configuration["DB:Name"]);

            //services.AddIdentity<Benutzer, Role>()
            //.AddRoleManager
            //.AddUserStore<RavenUserProvider>()
            //.AddRoleStore<RavenRoleStore>();
            //services.AddIdentityCore<Benutzer>()
            //    .AddUserStore<RavenUserProvider>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {

                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

                    var baseOnCreatingTicket = options.Events.OnCreatingTicket;
                    options.Events.OnCreatingTicket = async ctx =>
                    {
                        await baseOnCreatingTicket(ctx);
                        var id = ctx.User["id"].ToString();

                        using (var documentSession = documentStore.OpenSession())
                        {
                            var benutzer = documentSession.Query<Benutzer>().FirstOrDefault(b => b.GoogleLogin == id && b.IstAktiv);

                            if (benutzer != null)
                            {
                                foreach (var claim in ctx.Identity.Claims.ToList())
                                {
                                    ctx.Identity.RemoveClaim(claim);
                                }
                                var claims = new List<Claim> {
                                    new Claim(ClaimTypes.NameIdentifier, benutzer.EMail),
                                    new Claim(ClaimTypes.Name, benutzer.Name),
                                    new Claim(ClaimTypes.Email, benutzer.EMail)
                                };
                                foreach (var role in benutzer.Gruppen)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, role));
                                }
                                ctx.Identity.AddClaims(claims);
                            }
                        }
                    };
                })
                .AddTwitter(options =>
                {
                    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];

                    var baseOnCreatingTicket = options.Events.OnCreatingTicket;
                    options.Events.OnCreatingTicket = async ctx =>
                    {
                        await baseOnCreatingTicket(ctx);
                        var id = ctx.UserId;

                        using (var documentSession = documentStore.OpenSession()) {
                            var benutzer = documentSession.Query<Benutzer>().FirstOrDefault(b => b.TwitterLogin == id && b.IstAktiv);

                            if (benutzer != null) {
                                if (ctx.Principal != null) {
                                    //foreach (var claim in ctx.Principal.Claims.ToList()) {
                                    //    ctx.Principal. .Claims.RemoveClaim(claim);
                                    //}
                                }

                                //var claims = new List<Claim> {
                                //    new Claim(ClaimTypes.NameIdentifier, benutzer.EMail),
                                //    new Claim(ClaimTypes.Name, benutzer.Name),
                                //    new Claim(ClaimTypes.Email, benutzer.EMail)
                                //};
                                //foreach (var role in benutzer.Gruppen) {
                                //    claims.Add(new Claim(ClaimTypes.Role, role));
                                //}
                                //ctx.Identity.AddClaims(claims);
                            }
                        }
                    };
                });

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"],
                LanguageCode = "de"
            });

            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(Configuration["Rollbar:AccessToken"]));
            services.AddRollbarLogger(options =>
            {
                options.Filter = (loggerName, logLevel) => logLevel >= LogLevel.Error;
            });

            services.AddClock();
            services.AddFlickr(Configuration["Flickr:ApiKey"]);
            services.AddTwitter(Configuration["Twitter:ConsumerKey"], Configuration["Twitter:ConsumerSecret"], Configuration["Twitter:AccessToken"], Configuration["Twitter:AccessSecret"]);
            services.AddBackupToken(Configuration["Backup:Secret"]);

            services.AddAntiforgery();
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
                app.UseRollbarMiddleware();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }

    }

    public static class AddStartupExtensions
    {
        public static IDocumentStore AddRavenDB(this IServiceCollection services, string url, string database, string certificateString = null)
        {
            var urls = new string[] { url };
            var certificate = certificateString != null ? new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(certificateString)) : null;

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
