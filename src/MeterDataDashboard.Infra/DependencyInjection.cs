﻿using MeterDataDashboard.Core.MeterData.Services;
using MeterDataDashboard.Core.ScadaData.Services;
using MeterDataDashboard.Core.ScheduleData.Services;
using MeterDataDashboard.Infra.Identity;
using MeterDataDashboard.Infra.Persistence;
using MeterDataDashboard.Infra.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4;
using System;
using System.Collections.Generic;
using System.Text;
using MeterDataDashboard.Core.PmuData;
using MeterDataDashboard.Core.PmuData.Services;
using MeterDataDashboard.Core.ReportsData;
using MeterDataDashboard.Core.ReportsData.Services;
using MeterDataDashboard.Infra.Services.Email;
using MeterDataDashboard.Infra.Services.Sms;
using MeterDataDashboard.Infra.Services.ScadaNodes;
using MeterDataDashboard.Core.Sms;
using MeterDataDashboard.Core.ScadaNodes.Services;
using MeterDataDashboard.Infra.Services.TempHumidity;
using MeterDataDashboard.Core.TempHumidity.Services;
using MySql.Data.MySqlClient;

namespace MeterDataDashboard.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsEnvironment("Testing"))
            {
                // Add Persistence Infra
                services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "MeterData"));
                // Add Identity Infra
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "IdentityData"));
            }
            else
            {
                // Add Persistence Infra
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection")));

                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 2;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                // configure login path for return urls
                // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

            // Add meter data fetch service
            services.AddSingleton<IMeterDataService, MeterDataService>();
            services.AddSingleton<IScadaDataService, ScadaDataService>();
            services.AddSingleton<IWbesArchiveDataService, WbesArchiveDataService>();
            services.AddSingleton<IAgcFileUtilsService, AgcFileUtilsService>();
            services.AddSingleton<IWbesLiveDataService, WbesLiveDataService>();
            services.AddSingleton<IScadaNodesPingStatsService, ScadaNodesPingStatsService>();

            // add pmu config
            PmuConfig pmuConfig = new PmuConfig();
            configuration.Bind("PmuConfig", pmuConfig);
            services.AddSingleton(pmuConfig);

            // add reports data config
            ReportsConfig reportsConfig = new ReportsConfig();
            configuration.Bind("ReportsData", reportsConfig);
            services.AddSingleton(reportsConfig);

            // add pmu data service
            services.AddSingleton<IPMUHistDataService, PMUHistDataService>();

            // add reports data service
            services.AddSingleton<IReportsDataService, ReportsDataService>();

            // add email settings from app config
            EmailConfiguration emailConfig = new EmailConfiguration();
            configuration.Bind("EmailSettings", emailConfig);
            services.AddSingleton(emailConfig);

            // add sms settings from app config
            SmsConfiguration smsConfig = new SmsConfiguration();
            configuration.Bind("SmsSettings", smsConfig);
            services.AddSingleton(smsConfig);

            // Add Infra services
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();

            // Add temp monitoring realtime service
            services.AddSingleton<IDeviceDataService, DeviceDataService>();

            return services;
        }
    }
}
