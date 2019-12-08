﻿using MeterDataDashboard.Infra.Identity;
using MeterDataDashboard.Infra.Persistence;
using MeterDataDashboard.Infra.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsEnvironment("Testing"))
            {
                // Add Persistence Infra
                services.AddDbContext<MeterDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "MeterData"));
                // Add Identity Infra
                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "IdentityData"));
            }
            else
            {
                // Add Persistence Infra
                services.AddDbContext<MeterDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MeterDataDashboard.Infra")));

                services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MeterDataDashboard.Infra")));
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

            // Add Infra services
            services.AddTransient<IEmailSender, EmailSender>();
            return services;
        }
    }
}
