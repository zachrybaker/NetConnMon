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
using System.Text.Json;

namespace NetConnMon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OverridableSettings settings = null;
            if (File.Exists(AppSettingsUpdater.SettinsgOverridesFileName))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<OverridableSettings>(
                        File.ReadAllText(AppSettingsUpdater.SettinsgOverridesFileName));
                }
                catch(Exception e)
                {
                    Console.Write("JSON settings file has been messed up.  Please correct!");

                }
                // writes back any defaults that are missing.
                AppSettingsUpdater.UpdateSettings(settings);
            }
            else
            { 
                settings = new OverridableSettings()
                {
                    EncryptionSettings = Encryptor.CreateNewEncryptionSettings(),
                    SettingsVersion = new SettingsVersion() { Version = 1, Minor = 1 }
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
