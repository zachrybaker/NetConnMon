using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnMon.Domain.Configuration
{
    public class OverridableSettings
    {
        public SettingsVersion SettingsVersion { get; set; }
        public EncryptionSettings EncryptionSettings { get; set;  }
        public bool EnableHttpsUIRedirect { get; set; }
    }
}
