using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class FreezeMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private MembershipInstance? _membership;
    private int _membershipId;
    readonly WebService webService;
    public FreezeMembershipViewModel(WebService webService)
    {
        this.webService = webService;
        //  InitializeAsync();
    
       

    }

    [ObservableProperty]
    private DateTime _maximumDate;
   

    public int MembershipId
    {
        get { return _membershipId; }
        set
        {
            _membershipId = value;
            OnPropertyChanged(nameof(MembershipId));

            // Load the Membership when the MembershipId is set
            LoadMembership();
        }
    }

    private async Task LoadMembership()
    {
        try
        {
           // Debug.WriteLine("MembershipId: " + MembershipId);
            Membership = await webService.GetMembershipInstanceById(MembershipId);
            MaximumDate = DateTime.Today.AddDays(Membership.ActiveFreeze.DaysLeft);
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
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("//ProfileView");
    }

    [RelayCommand]
    private async Task FreezeAsync()
    {
        try
        {
          //  await webService.FreezeMembershipInstance(MembershipId);
            await Shell.Current.DisplayAlert("Success", "Membership is frozen", "Ok");
            await Shell.Current.GoToAsync("//ProfileView");
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


}