using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.ViewModels;

public class VehicleDetailsViewModel
{
    public string Name { get; set; }
    public DateTimeOffset? FirstIgnitionOn { get; set; }
    public DateTimeOffset? LastIgnitionOff { get; set; }
    public double? HoursBetweenFirstOnAndLastOff { get; set; }
    public double? DailyMileage { get; set; }

    public string FirstIgnitionOnPoland => FirstIgnitionOn?.ToString("HH:mm");
    public string LastIgnitionOffPoland => LastIgnitionOff?.ToString("HH:mm");

    public static VehicleDetailsViewModel FromDto(VehicleDetailsDto dto) => new()
    {
        Name = dto.Name,
        FirstIgnitionOn = dto.FirstIgnitionOn,
        LastIgnitionOff = dto.LastIgnitionOff,
        HoursBetweenFirstOnAndLastOff = dto.HoursBetweenFirstOnAndLastOff,
        DailyMileage = dto.DailyMileage
    };
}
