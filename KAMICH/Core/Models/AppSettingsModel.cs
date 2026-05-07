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
        public VehicleStatsModel GlobalVehicleSettings { get; set; }
        
    }
}
