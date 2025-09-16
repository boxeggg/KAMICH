using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Integrations.Linqo.Models
{
    public class VehicleListModel
    {
        public List<VehicleModelDto> Vehicles { get; set; } = new List<VehicleModelDto>();
    }
}
