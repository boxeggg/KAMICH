using KAMICH.Core.Services;
using KAMICH.Integrations.Linqo;
using KAMICH.Integrations.Linqo.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KAMICH.Pages
{
    public partial class FleetPage : ContentPage
    {
        private readonly IVehicleService _vehicleService;
        private readonly IMemoryService _memoryService;

        public ObservableCollection<VehicleModelDto> Vehicles { get; } = new();
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
                    if (Guid.TryParse(idStr, out var parsed))
                    {
                        await Shell.Current.GoToAsync($"details?VehicleId={idStr}");
                    }
                }
                catch(Exception ex)
                {
                    try { await DisplayAlert("Błąd", "Nie udało się otworzyć szczegółów pojazdu."+ex,  "OK"); } catch { }
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
            await LoadAsync();
        }
        private async void OnRefreshIconTapped(object sender, EventArgs e) => await LoadAsync(false);

        private async Task LoadAsync(bool withCache = true)
        {
            ErrorLabel.IsVisible = false;
            NoDataLabel.IsVisible = false;
            Loader.IsVisible = true;
            Loader.IsRunning = true;

            Vehicles.Clear();

            try
            {
                VehicleListModel result;
                if (withCache)
                {
                    result = await _vehicleService.GetVehicles();
                }
                else
                {
                    result = await _vehicleService.GetVehicles(false);
                }

                if (result == null || result.Vehicles == null || !result.Vehicles.Any())
                {
                    NoDataLabel.IsVisible = true;
                    return;
                }

                foreach (var v in result.Vehicles.OrderBy(v => v.Name ?? string.Empty))
                {
                    Vehicles.Add(v);
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
                catch { /* ignore UI failure */ }
            }
            finally
            {
                Loader.IsRunning = false;
                Loader.IsVisible = false;
            }
        }
    }
}