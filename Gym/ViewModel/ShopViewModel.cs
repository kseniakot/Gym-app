using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Runtime.CompilerServices;
using System.Diagnostics;


namespace Gym.ViewModel;

public partial class ShopViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Membership> _memberships;

    //[ObservableProperty]
    //private ObservableCollection<Freeze> _freezes;

    [ObservableProperty]
    private Membership _selectedMembership;


    //[ObservableProperty]
    //private Freeze _selectedFreeze;

    [ObservableProperty]
    bool _isMembershipPickerVisible;

    //[ObservableProperty]
    //bool _isFreezePickerVisible;


    readonly WebService webService;
    public ShopViewModel(WebService webService)
    {
        this.webService = webService;
       InitializeAsync();
        if (Memberships != null && Memberships.Count > 0) SelectedMembership = Memberships.First();
       // if (Freezes != null && Freezes.Count > 0) SelectedFreeze = Freezes.First();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Memberships = new ObservableCollection<Membership>(await webService.GetAllMemberships());
           // Freezes = new ObservableCollection<Freeze>(await webService.GetAllFreezes());
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
    private void ToggleMembershipPicker()
    {
        IsMembershipPickerVisible = !IsMembershipPickerVisible;
       
    }

    //[RelayCommand]
    //private void ToggleFreezePicker()
    //{
       
    //    IsFreezePickerVisible = !IsFreezePickerVisible;
    //}



    [RelayCommand]
    private async Task ViewMembershipAsync()
    {
       if (SelectedMembership == null) return;
        await Shell.Current.GoToAsync($"MembershipView?MembershipId={SelectedMembership.Id}");
        Debug.WriteLine(SelectedMembership.Id);
           // SelectedMembership = null;
        
       

    }
}