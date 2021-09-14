using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnMon.Domain.Configuration
{
    public class EncryptionSettings
    {
        public string KeyBase64 { get; set; } = "";
        public string IVBase64 { get; set; } = "";
    }
}
