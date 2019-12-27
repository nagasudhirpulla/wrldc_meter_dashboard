using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using MeterDataDashboard.Infra.Identity;
using MeterDataDashboard.Infra.Persistence;
using MeterDataDashboard.Infra.Services;
using MeterDataDashboard.Infra;
using MeterDataDashboard.Application.Common;
using MeterDataDashboard.Web.Services;

namespace MeterDataDashboard.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration, Environment);
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services
                .AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/ScadaArchiveMeasurements");
            });

            // add identity server authentication
            // https://stackoverflow.com/questions/39864550/how-to-get-base-url-without-accessing-a-request
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:58280/";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "scada_archive";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseIdentityServer();
            app.UseAuthorization();

            // seed Users and Roles
            AppIdentityInitializer identityInitializer = new AppIdentityInitializer()
            {
                UserManager = userManager,
                RoleManager = roleManager,
                Configuration = Configuration
            };
            identityInitializer.SeedData();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
