using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Runtime.CompilerServices;

namespace Gym.ViewModel;

public partial class ShopViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Membership> _memberships;
    [ObservableProperty]
    private Membership _selectedMembership;

    [ObservableProperty]
    bool _isPickerVisible;
   

    readonly WebService webService;
    public ShopViewModel(WebService webService)
    {
        this.webService = webService;
       InitializeAsync();
        if (Memberships != null && Memberships.Count > 0) SelectedMembership = Memberships.First();
    }

    private async Task InitializeAsync()
    {
        try
        {
            Memberships = new ObservableCollection<Membership>(await webService.GetAllMemberships());
        }
        catch (SessionExpiredException)
        {
            await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
            await Shell.Current.GoToAsync("SignInView");
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
        }
    }

    [RelayCommand]
    private void TogglePicker()
    {
        IsPickerVisible = !IsPickerVisible;
    }

}