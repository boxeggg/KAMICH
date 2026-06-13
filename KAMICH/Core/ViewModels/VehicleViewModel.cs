using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.ViewModels;

public class VehicleViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.FromRgb(128, 128, 128);
    public string Icon { get; set; } = "";

    public static VehicleViewModel FromDto(VehicleModelDto dto, MemoryVehicleDetails? memory)
    {
        var vm = new VehicleViewModel
        {
            Id = dto.Id,
            Name = dto.Name
        };

        if (memory != null)
        {
            vm.Color = memory.CustomColor;
            vm.Icon = memory.CustomIcon;
        }

        return vm;
    }
}
