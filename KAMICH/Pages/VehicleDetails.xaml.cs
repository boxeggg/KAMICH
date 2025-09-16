using System;
using System.Threading.Tasks;
using KAMICH.Core.Services;
using KAMICH.Integrations.Linqo.Models;
using Microsoft.Maui.Controls;

namespace KAMICH.Pages;

public partial class VehicleDetails : ContentPage
{
    private readonly Guid _vehicleId;
    private readonly IVehicleService _vehicleService;

    public VehicleDetails(Guid vehicleId, IVehicleService vehicleService)
    {
        _vehicleId = vehicleId;
        _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        InitializeComponent();


        BindingContext = new VehicleDetailsViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {

            var localToday = DateTime.Now.Date;
            var from = new DateTimeOffset(localToday, DateTimeOffset.Now.Offset);
            var to = DateTimeOffset.Now;

            var vm = await _vehicleService.GetVehiclesDetails(_vehicleId, from, to);

            if (vm != null)
            {
                BindingContext = vm;
            }
        }
        catch (Exception ex)
        {
            try
            {
                await DisplayAlert("Błąd", $"Nie udało się pobrać danych: {ex.Message}", "OK");
            }
            catch
            {

            }
        }
    }
}