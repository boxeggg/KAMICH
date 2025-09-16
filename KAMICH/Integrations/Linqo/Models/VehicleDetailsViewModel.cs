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
        public bool? isTracked { get; set; }
        public double? FuelPrice { get; set; }
        public double? HourlyPrice { get; set; }
        public double? OperatorPrice { get; set; }
        public Color? CustomColor { get; set; }
        public VehicleIcon? CustomIcon { get; set; }
    }

    public enum VehicleIcon
    {
        Excavator = 0xF7D9,   
        Truck = 0xF0D1,       
        Tractor = 0xF722,     
        Bulldozer = 0xF7DC,    
        Loader = 0xF7DE,      
        DumpTruck = 0xF7DD,   
        Crane = 0xF7D6,       
        PickupTruck = 0xF63B, 
        Van = 0xF7DF          
    }
}
