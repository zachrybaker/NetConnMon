using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace NetConnMon.Domain.Configuration
{
    public static class AppSettingsUpdater
    {
        // KISS: https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json
        public static void UpdateSettings(OverridableSettings settings)
        {
            // instead of updating appsettings.json file directly I will just write the part I need to update to appsettings.MyOverrides.json
            // .Net Core in turn will read my overrides from appsettings.MyOverrides.json file
            var newConfig = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettinsgOverridesFileName, newConfig);
        }
        public static string SettinsgOverridesFileName => Path.Combine(Config.dockerDBVolumnePath, Config.appSettingOverrideFile);
    }
}
