using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Identity;
//using Microsoft.Extensions.Identity.Core;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetConnMon.Persistence.DBContexts;
using NetConnMon.Persistence.Repos;

namespace NetConnMon.Persistence
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IdentityBuilder identityBuilder)
        {
            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                        configuration.GetConnectionString("DefaultConnection")))

                .AddDbContext<TestDbContext>(options =>
                    options
                    .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                    .UseBatchEF_Sqlite());
            identityBuilder
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<TestDbContext>();

            // doesn't seem to handle our interface generic specifications.
            //https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
            // https://github.com/khellang/Scrutor
            services.AddRepositories();

            return services;
        }
    }
}
