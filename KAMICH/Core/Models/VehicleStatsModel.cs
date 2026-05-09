using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Core.Models
{
    public record VehicleStatsModel
    {
        public double FuelPrice { get; set; }
        public double HourlyPrice { get; set; }
        public double OperatorPrice { get; set; }
        public double FuelConsumptionPerHour { get; set; }
        public FuelConsumptionType FuelConsumptionMode { get; set; } = FuelConsumptionType.Normal;
    }
    public enum FuelConsumptionType
    {
        Heavy,Normal,Eco
    }


}
