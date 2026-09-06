using System;
using System.Threading.Tasks;
using KAMICH.Core.Services;
using KAMICH.Core.ViewModels;
using KAMICH.Exceptions;
using Microsoft.Maui.Controls;

namespace KAMICH.Pages;

[QueryProperty(nameof(VehicleId), "VehicleId")]
public partial class VehicleDetails : ContentPage
{
    private Guid _vehicleId;
    private readonly IVehicleService _vehicleService;
    private readonly IMemoryService _memoryService;
    private readonly IErrorHandler _errors;
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

    public VehicleDetails(IVehicleService vehicleService, IMemoryService memoryService, IErrorHandler errors)
    {
        _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        _memoryService = memoryService ?? throw new ArgumentNullException(nameof(memoryService));
        _errors = errors ?? throw new ArgumentNullException(nameof(errors));
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

    private async void OnStatsTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"stats?VehicleId={_vehicleId}");
    }


    private async Task LoadDataAsync(Guid id)
    {
        // No inline error surface on this page, so the user gets an alert.
        await _errors.SafeRunAsync(async () =>
        {
            var localToday = DateTime.Now.Date;
            var from = new DateTimeOffset(localToday, DateTimeOffset.Now.Offset);
            var to = DateTimeOffset.Now;

            var dto = await _vehicleService.GetVehiclesDetails(id, from, to);
            if (dto != null)
            {
                BindingContext = VehicleDetailsViewModel.FromDto(dto);
            }
        }, "VehicleDetails.LoadDataAsync", ErrorPolicy.Notify);
    }
}