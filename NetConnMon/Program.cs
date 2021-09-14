using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetConnMon.Domain.Configuration;
using System.IO;
using NetConnMon.Domain.Logic;

namespace NetConnMon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists(AppSettingsUpdater.SettinsgOverridesFileName))
            {
                var settings = new OverridableSettings()
                {
                    EncryptionSettings = Encryptor.CreateNewEncryptionSettings(),
                    SettingsVersion = new SettingsVersion() { Version = 1 }
                };
                AppSettingsUpdater.UpdateSettings(settings);
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((env, config) =>
                {
                    config.AddJsonFile(AppSettingsUpdater.SettinsgOverridesFileName, optional: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseDefaultServiceProvider(options => options.ValidateScopes = false); // needed for mediatr DI
    }
}
