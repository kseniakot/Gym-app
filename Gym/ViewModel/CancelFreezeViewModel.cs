using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class CancelFreezeViewModel : ObservableObject
{
    [ObservableProperty]
    private MembershipInstance? _membership;
    private int _membershipId;
    readonly WebService webService;
    public CancelFreezeViewModel(WebService webService)
    {
        this.webService = webService;
        //  InitializeAsync();

    }

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
            Membership = await webService.GetMembershipInstanceById(MembershipId);
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
    private async Task ActivateAsync()
    {
        try
        {
            if ((Membership.ActiveFreeze.StartDate - DateTime.Today.ToUniversalTime()).Value.Days < Membership.Membership.Freeze.MinDays)

            {

                await Shell.Current.DisplayAlert("Failed", $"Less than {Membership.Membership.Freeze.MinDays} days passed", "Ok");
            }
            else
            {
                //Debug.WriteLine((Membership.ActiveFreeze.EndDate - DateTime.Today.ToUniversalTime()).Value.Days);
                await webService.CancelFreeze(Membership.Id);
                await Shell.Current.DisplayAlert("Success", "Freeze is cancelled", "Ok");
           }
            
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