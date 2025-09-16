using KAMICH.Integrations.Linqo;
using KAMICH.Integrations.Linqo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Core.Services
{
    public interface IVehicleService
    {
        Task<VehicleListModel> GetVehicles(bool UseCache = true, CancellationToken ct = default);
        Task<VehicleDetailsViewModel> GetVehiclesDetails(Guid objectId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    }
}
