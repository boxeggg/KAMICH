using System;
using System.Collections.Generic;
using System.Text;

namespace KAMICH.Integrations.Linqo.Models
{
    public class VersionInfoDto
    {
        public string VersionName { get; set; } = string.Empty;
        public bool IsCached { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
