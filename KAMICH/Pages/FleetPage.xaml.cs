using KAMICH.Core.Services;
using KAMICH.Core.ViewModels;
using KAMICH.Exceptions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KAMICH.Pages;

public partial class FleetPage : ContentPage
{
    private readonly IVehicleService _vehicleService;
    private readonly IMemoryService _memoryService;
    private readonly IErrorHandler _errors;

    public ObservableCollection<VehicleViewModel> Vehicles { get; } = new();
    public ICommand NavigateToVehicleCommand { get; }
    private bool _isNavigating;

    public FleetPage(IVehicleService vehicleService, IMemoryService memoryService, IErrorHandler errors)
    {
        InitializeComponent();
        _vehicleService = vehicleService;
        _memoryService = memoryService;
        _errors = errors;
        BindingContext = this;

        NavigateToVehicleCommand = new Command<object>(async param =>
        {
            if (_isNavigating) return;
            _isNavigating = true;

            await _errors.SafeRunAsync(async () =>
            {
                if (param == null) return;

                var idStr = param.ToString();
                if (Guid.TryParse(idStr, out _))
                {
                    await Shell.Current.GoToAsync($"details?VehicleId={idStr}");
                }
            }, "FleetPage.NavigateToVehicle", ErrorPolicy.Notify);

            _isNavigating = false;
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync(withCacheBypass: false);
    }

    private async void OnRefreshIconTapped(object sender, EventArgs e) => await LoadAsync(withCacheBypass: true);

    private async Task LoadAsync(bool withCacheBypass)
    {
        ErrorLabel.IsVisible = false;
        NoDataLabel.IsVisible = false;
        Loader.IsVisible = true;
        Loader.IsRunning = true;

        Vehicles.Clear();

        // Logged by policy; the page shows the same wording inline instead of an alert.
        var error = await _errors.SafeRunAsync(async () =>
        {
            var vehicles = await _vehicleService.GetVehicles(bypassCache: withCacheBypass);

            if (vehicles == null || vehicles.Count == 0)
            {
                NoDataLabel.IsVisible = true;
                return;
            }

            foreach (var v in vehicles.OrderBy(v => v.Name ?? string.Empty))
            {
                var memory = await _memoryService.GetMemoryVehiclesDetails(v.Id);
                Vehicles.Add(VehicleViewModel.FromDto(v, memory));
            }
        }, "FleetPage.LoadAsync");

        if (error is not null)
        {
            ErrorLabel.Text = error.Message;
            ErrorLabel.IsVisible = true;
        }

        Loader.IsRunning = false;
        Loader.IsVisible = false;
    }
}
