using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Core.Models
{
    public class AppSettingsModel
    {
        public bool DarkMode { get; set; }
        public string? ApiKey { get; set; }
        
        public double GlobalFuelPrice { get; set; }
        
        public double GlobalHourlyPrice { get; set; }
        
        public double GlobalOperatorPrice { get; set; }
        
    }
}
