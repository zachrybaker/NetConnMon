using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnMon.Server.Configuration
{
    public class EncryptionSettings
    {
        public string Key { get; set; } = "";
        public string IV { get; set; }
    }
}
