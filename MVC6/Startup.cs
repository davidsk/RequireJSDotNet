using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequireJsNet.EntryPointResolver;

namespace MVC6
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Map("/application", (app1) => this.Configure1(app1, env, loggerFactory));
            //this.Configure1(app, env, loggerFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure1(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                // Areas support
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new {controller = "home", action = "index"}
                    );
            });

            app.Run(async (context) =>
            {
                var urlPatterns = new Dictionary<string, string>();
                urlPatterns.Add("hostRelative", "/bar");
                urlPatterns.Add("appRelativeExplicit", context.Request.PathBase + "/bar");
                urlPatterns.Add("appRelativeImplicit", "~/bar");
                urlPatterns.Add("relative", "../bar");

                var message = string.Empty;

                foreach (var kvp in urlPatterns)
                {
                    message +=
                        $"{kvp.Key}: {DefaultEntryPointResolver.ResolvePathToAppRoot(kvp.Value, context.Request.PathBase)}\n";
                }

                await context.Response.WriteAsync(message);
            });
        }
    }
}
