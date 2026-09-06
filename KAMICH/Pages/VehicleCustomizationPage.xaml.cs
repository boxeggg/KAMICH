using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KAMICH.Core.Models;
using KAMICH.Core.Services;
using KAMICH.Exceptions;

namespace KAMICH.Pages;

[QueryProperty(nameof(VehicleId), "VehicleId")]
public partial class VehicleCustomizationPage : ContentPage
{
    private Guid _vehicleId;
    private readonly IMemoryService _memoryService;
    private readonly IVehicleService _vehicleService;
    private readonly IErrorHandler _errors;
    public ObservableCollection<IconItem> Icons { get; set; } = new();
    public ObservableCollection<Color> Colors { get; set; } = new();

    public IconItem SelectedIcon { get; set; }
    public Color SelectedColor { get; set; }
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
    public VehicleCustomizationPage(IMemoryService memoryService, IVehicleService vehicleService, IErrorHandler errors)
    {
        _memoryService = memoryService;
        _vehicleService = vehicleService;
        _errors = errors;
        InitializeComponent();
        Icons.Add(new IconItem { IconGlyph = "\uf1b9" });
        Icons.Add(new IconItem { IconGlyph = "\uf0d1" }); 
        Icons.Add(new IconItem { IconGlyph = "\uf4df" }); 
        Icons.Add(new IconItem { IconGlyph = "\uf48b" }); 
        Icons.Add(new IconItem { IconGlyph = "\uf722" }); 
        Colors.Add(Color.FromRgb(255, 0, 0));    
        Colors.Add(Color.FromRgb(0, 0, 255));    
        Colors.Add(Color.FromRgb(0, 128, 0));    
        Colors.Add(Color.FromRgb(255, 165, 0));  
        Colors.Add(Color.FromRgb(128, 128, 128));
        
        BindingContext = this;
        
        IconCollection.SelectionChanged += IconCollection_SelectionChanged;
        ColorCollection.SelectionChanged += ColorCollection_SelectionChanged;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var vm = await _memoryService.GetMemoryVehiclesDetails(_vehicleId);

        if (vm != null)
        {
            SelectedIcon = Icons.FirstOrDefault(x => x.IconGlyph == vm.CustomIcon) 
                           ?? Icons.First();

            SelectedColor = Colors.FirstOrDefault(c => Equals(c, vm.CustomColor))
                            ?? Colors.First();
            HourlyRateEntry.Text = vm.HourlyPrice.ToString() ?? "";
            OperatorRateEntry.Text = vm.OperatorPrice.ToString() ?? "";
            FuelPriceEntry.Text = vm.FuelPrice.ToString() ?? "";
            FuelConsumptionEntry.Text = vm.FuelConsumptionPerHour.ToString() ?? "";
            TrackingSwitch.IsToggled = vm.IsTracked;

        }
        else
        {
            SelectedIcon = Icons[0];
            SelectedColor = Colors[1];
        }
        
        

        IconCollection.ItemsSource = Icons;
        IconCollection.SelectedItem = SelectedIcon;

        ColorCollection.ItemsSource = Colors;
        ColorCollection.SelectedItem = SelectedColor;
    }
    private void IconCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            SelectedIcon = e.CurrentSelection[0] as IconItem;
        }
    }

    private void ColorCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            SelectedColor = (Color)e.CurrentSelection[0];
        }
    }

    private async void SaveButton(object? sender, EventArgs e) =>
        await _errors.SafeRunAsync(SaveAsync, "VehicleCustomizationPage.Save", ErrorPolicy.Notify);

    private async Task SaveAsync()
    {
        var model = await _memoryService.GetMemoryVehiclesDetails(_vehicleId);
        var vehicles = await _vehicleService.GetVehicles();
        if (model != null)
        {
            model.Name = vehicles.FirstOrDefault(x => x.Id == _vehicleId)?.Name;
            model.CustomColor = SelectedColor;
            model.CustomIcon = SelectedIcon.IconGlyph;
            model.FuelPrice = double.TryParse((FuelPriceEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var g)
                ? g
                : 0.0;
            model.OperatorPrice = double.TryParse((OperatorRateEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var d)
                ? d
                : 0.0;
            model.HourlyPrice = double.TryParse((HourlyRateEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var f)
                ? f
                : 0.0;
            model.FuelConsumptionPerHour = double.TryParse((FuelConsumptionEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var fc)
                ? fc
                : 0.0;
            model.IsTracked = TrackingSwitch.IsToggled;
            if(await _memoryService.SetMemoryVehicle(model))
            {
                await _vehicleService.GetVehicles(bypassCache: true);
                await Shell.Current.GoToAsync($"..");
            }
            else await DisplayAlert("Błąd", "Nie udało się zapisać ustawień pojazdu.", "OK");

        }
        else
        {
            var newModel = new MemoryVehicleDetails
            {
                Name = vehicles.FirstOrDefault(x => x.Id == _vehicleId)?.Name,
                Id = _vehicleId,
                CustomColor = SelectedColor,
                CustomIcon = SelectedIcon.IconGlyph,
                IsTracked = TrackingSwitch.IsToggled,
                FuelPrice = double.TryParse((FuelPriceEntry?.Text ?? "").Trim().Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var fp) ? fp : 0.0,
                HourlyPrice = double.TryParse((HourlyRateEntry?.Text ?? "").Trim().Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var hp) ? hp : 0.0,
                OperatorPrice = double.TryParse((OperatorRateEntry?.Text ?? "").Trim().Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var op) ? op : 0.0,
                FuelConsumptionPerHour = double.TryParse((FuelConsumptionEntry?.Text ?? "").Trim().Replace(',', '.'),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out var fcp) ? fcp : 0.0
            };
            if(await _memoryService.SetMemoryVehicle(newModel)) await Shell.Current.GoToAsync($"..");
            else await DisplayAlert("Błąd", "Nie udało się zapisać ustawień pojazdu.", "OK");
        }

    }
}
public class IconItem
{
    public string IconGlyph { get; set; }
}