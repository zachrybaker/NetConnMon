using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnMon.Domain.Configuration
{
    public static class Config
    {
        // just going to support sqlite until we know we should go to mariahdb or the like.  Perhaps should be totally configurable...in time.
        public const string dbFilename = @"app.db";

        public const string appSettingOverrideFile = "appsettings.overrides.json";
        public const string dockerDBVolumnePath = @"netconnmon-db";
    }
}
