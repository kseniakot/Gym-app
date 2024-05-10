using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
using Microsoft.Maui.Controls;
//using Xamarin.KotlinX.Coroutines.Channels;
using Microsoft.Maui.Controls;
using Gym.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class ProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private ImageSource? _myImageSource;
    [ObservableProperty]
    private User _user;
    [ObservableProperty]
    private ObservableCollection<MembershipInstance> _activeMemberships;
    [ObservableProperty]
    private ObservableCollection<MembershipInstance> _notActiveMemberships;
    [ObservableProperty]
    private MembershipInstance _selectedMembership;

    readonly WebService webService;
    public ProfileViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();
      
    }

    private async Task InitializeAsync()
    {
        
          User = await webService.GetUserFromToken();
        MyImageSource = ImageSource.FromFile("User.png");

        try
        {
            ActiveMemberships = new ObservableCollection<MembershipInstance>(await webService.GetActiveMembershipsByUserId(User.Id));
            NotActiveMemberships = new ObservableCollection<MembershipInstance>(await webService.GetNotActiveMembershipsByUserId(User.Id));
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
    private async Task LoadData()
    {
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task SelectImage()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick Image Please",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
            return;

        var stream = await result.OpenReadAsync();

       MyImageSource = ImageSource.FromStream(() => stream);

    }

    [RelayCommand]
    private async Task ActivateMembershipAsync()
    {
        if (SelectedMembership == null) return;
        await Shell.Current.GoToAsync($"ActivateMembershipView?MembershipId={SelectedMembership.Id}");

    }

    [RelayCommand]
    private async Task FreezeMembershipAsync()
    {
        if (SelectedMembership == null) return;
        Debug.WriteLine("FreezeMembershipAsync" + SelectedMembership.Id);

        await Shell.Current.GoToAsync($"FreezeMembershipView?MembershipId={SelectedMembership.Id}");
       
    }
}