using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using Gym.Model;
using Gym.Services;

namespace Gym.ViewModel;

public partial class ShopViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Membership> _memberships;
    [ObservableProperty]
    private Membership _selectedMembership;

    [ObservableProperty]
    bool _isPickerVisible;
   

    readonly DataBaseService _dbService;
    public ShopViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
        Memberships = new ObservableCollection<Membership>(dbService.GetAllMemberships());
        SelectedMembership = Memberships.First();
    }

    [RelayCommand]
    private void TogglePicker()
    {
        IsPickerVisible = !IsPickerVisible;
    }

}