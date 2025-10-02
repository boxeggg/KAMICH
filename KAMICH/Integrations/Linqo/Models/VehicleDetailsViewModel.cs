using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Integrations.Linqo.Models
{
    public class VehicleDetailsViewModel
    {
        public string Name { get; set; }
        public DateTimeOffset? FirstIgnitionOn { get; set; }
        public DateTimeOffset? LastIgnitionOff { get; set; }
        public double? HoursBetweenFirstOnAndLastOff { get; set; }
        public double? DailyMileage { get; set; }
        public string FirstIgnitionOnPoland => FirstIgnitionOn?.ToString("HH:mm") ?? null;
        public string LastIgnitionOffPoland => LastIgnitionOff?.ToString("HH:mm") ?? null;
        public Color? CustomColor { get; set; }
        public string? CustomIcon { get; set; }
        
    }


}
