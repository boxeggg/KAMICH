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

        public ObservableCollection<VehicleModelDto> Vehicles { get; } = new();
        public ICommand NavigateToVehicleCommand { get; }
        private bool _isNavigating;

        public FleetPage(IVehicleService vehicleService)
        {
            InitializeComponent();
            _vehicleService = vehicleService;
            BindingContext = this;

            NavigateToVehicleCommand = new Command<object>(async param =>
            {
                if (_isNavigating) return;
                _isNavigating = true;

                try
                {
                    if (param == null) return;

                    if (param is Guid guid)
                    {
                        await Navigation.PushAsync(new VehicleDetails(guid, _vehicleService));
                        return;
                    }

                    var idStr = param.ToString();
                    if (Guid.TryParse(idStr, out var parsed))
                    {
                        await Navigation.PushAsync(new VehicleDetails(parsed, _vehicleService));
                    }
                }
                catch
                {
                    try { await DisplayAlert("Błąd", "Nie udało się otworzyć szczegółów pojazdu.", "OK"); } catch { }
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
        // zmieniamy na async żeby móc awaitować
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
                    Debug.WriteLine("LoadAsync: result or result.Vehicles is null/empty.");
                    return;
                }

                foreach (var v in result.Vehicles.OrderBy(v => v.Name ?? string.Empty))
                {
                    Vehicles.Add(v);
                }
            }
            catch (Exception ex)
            {
                // pokaż użytkownikowi i zaloguj dla devów — minimalny koszt, duża wartość
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