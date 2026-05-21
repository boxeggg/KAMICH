using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Integrations.Api
{
    public class HealthCheckDto
    {
        public List<string> Groups { get; set; }
        public string Status { get; set; }
    }
}
