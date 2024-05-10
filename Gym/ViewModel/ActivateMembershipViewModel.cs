using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class ActivateMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private MembershipInstance? _membership;
    private int _membershipId;
    readonly WebService webService;
    public ActivateMembershipViewModel(WebService webService)
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
            
            if (!(await webService.DoesActiveMembershipExist((await webService.GetUserFromToken()).Id)))
            {
                await webService.ActivateMembershipInstance(MembershipId);
                await Shell.Current.DisplayAlert("Success", "Membership activated", "Ok");
             }
            else
        {
                await Shell.Current.DisplayAlert("Warning", "You already have one ACTIVE membership", "Ok");
                bool answer = await Shell.Current.DisplayAlert("Question", "Activate anyway?", "Yes", "No");
                if (answer)
                {
                    await webService.ActivateMembershipInstance(MembershipId);
                    await Shell.Current.DisplayAlert("Success", "Membership activated", "Ok");
                }
               
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