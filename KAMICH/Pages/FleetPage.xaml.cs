using KAMICH.Core.Services;
using KAMICH.Core.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace KAMICH.Pages;

public partial class FleetPage : ContentPage
{
    private readonly IVehicleService _vehicleService;
    private readonly IMemoryService _memoryService;

    public ObservableCollection<VehicleViewModel> Vehicles { get; } = new();
    public ICommand NavigateToVehicleCommand { get; }
    private bool _isNavigating;

    public FleetPage(IVehicleService vehicleService, IMemoryService memoryService)
    {
        InitializeComponent();
        _vehicleService = vehicleService;
        _memoryService = memoryService;
        BindingContext = this;

        NavigateToVehicleCommand = new Command<object>(async param =>
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                if (param == null) return;

                var idStr = param.ToString();
                if (Guid.TryParse(idStr, out _))
                {
                    await Shell.Current.GoToAsync($"details?VehicleId={idStr}");
                }
            }
            catch (Exception ex)
            {
                try { await DisplayAlert("Błąd", "Nie udało się otworzyć szczegółów pojazdu." + ex, "OK"); } catch { }
            }
            finally
            {
                _isNavigating = false;
            }
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

        try
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadAsync exception: {ex}");
            try
            {
                ErrorLabel.Text = "Wystąpił błąd podczas pobierania listy pojazdów.";
                ErrorLabel.IsVisible = true;
            }
            catch { }
        }
        finally
        {
            Loader.IsRunning = false;
            Loader.IsVisible = false;
        }
    }
}
