using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KAMICH.Core.Models;
using KAMICH.Core.Services;

namespace KAMICH.Pages;

[QueryProperty(nameof(VehicleId), "VehicleId")]
public partial class VehicleCustomizationPage : ContentPage
{
    private Guid _vehicleId;
    private readonly IMemoryService  _memoryService;
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
    public VehicleCustomizationPage(IMemoryService memoryService)
    {
        _memoryService = memoryService;
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

    private async void SaveButton(object? sender, EventArgs e)
    {
        var model = await _memoryService.GetMemoryVehiclesDetails(_vehicleId);
        if (model != null)
        {
            model.CustomColor = SelectedColor;
            model.CustomIcon = SelectedIcon.IconGlyph;
            if(await _memoryService.SetMemoryVehicle(model)) await Shell.Current.GoToAsync($"..");
            else await DisplayAlert("Warning", "Something went wrong", "OK");

        }
        else
        {
            var newModel = new MemoryVehicleDetails
            {
                Id = _vehicleId,
                CustomColor = SelectedColor,
                CustomIcon = SelectedIcon.IconGlyph
            };
            if(await _memoryService.SetMemoryVehicle(newModel)) await Shell.Current.GoToAsync($"..");
            else await DisplayAlert("Warning", "Something went wrong", "OK");
        }

    }
}
public class IconItem
{
    public string IconGlyph { get; set; }
}