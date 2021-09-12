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
using System.IO;

namespace NetConnMon.Persistence
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IdentityBuilder identityBuilder)
        {
            CopyDbFileToVolumeMount();

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

        private static string dbFilename = "app.db";
        private static string dockerDBVolumnePath = "netconnmon-db";
        private static void CopyDbFileToVolumeMount()
        {
            var dest = Path.Combine(dockerDBVolumnePath, dbFilename);
            // We should already be in "app"
            if (!Directory.Exists(dockerDBVolumnePath))
                throw new NullReferenceException($"Must configure app to start in docker with the moun volume \"{dockerDBVolumnePath}\" specified");
            else if (!File.Exists(dbFilename))
                throw new NullReferenceException($"{dbFilename} is missing from executable directory.  Path is {Directory.GetCurrentDirectory()}");
            else if (!File.Exists(dest))
                File.Copy(dbFilename, dest);
        }
    }
}
