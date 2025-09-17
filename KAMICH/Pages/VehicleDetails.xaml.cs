using System;
using System.Threading.Tasks;
using KAMICH.Core.Services;
using KAMICH.Integrations.Linqo.Models;
using Microsoft.Maui.Controls;

namespace KAMICH.Pages;

[QueryProperty(nameof(VehicleId), "VehicleId")]
public partial class VehicleDetails : ContentPage
{
    private Guid _vehicleId;
    private readonly IVehicleService _vehicleService;
    private readonly IMemoryService _memoryService;
    public string VehicleId
    {
        get => _vehicleId.ToString();
        set
        {
            if (Guid.TryParse(value, out var parsed))
            {
                _vehicleId = parsed;
            }
        }
    }

    public VehicleDetails(IVehicleService vehicleService, IMemoryService memoryService)
    {
        _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        _memoryService = memoryService ?? throw new ArgumentNullException(nameof(memoryService));
        InitializeComponent();


        BindingContext = new VehicleDetailsViewModel();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await LoadDataAsync(_vehicleId);
    }

    private async void OnCustomizeTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"customization?VehicleId={_vehicleId}");
    }


    private async Task LoadDataAsync(Guid id)
    {
        try
        {
            var localToday = DateTime.Now.Date;
            var from = new DateTimeOffset(localToday, DateTimeOffset.Now.Offset);
            var to = DateTimeOffset.Now;

            var vm = await _vehicleService.GetVehiclesDetails(id, from, to);
            if (vm != null)
            {
                BindingContext = vm;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] LoadDataAsync: {ex}");
            try
            {
                await DisplayAlert("Błąd",
                    $"Nie udało się otworzyć szczegółów pojazdu.\n{ex.Message}",
                    "OK");
            }
            catch
            {
            }
        }
    }
}