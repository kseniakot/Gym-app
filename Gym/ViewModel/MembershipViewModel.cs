using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
//using Java.Util;
namespace Gym.ViewModel;

public partial class MembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership? _membership;

    private int _membershipId;
    readonly WebService webService;
    public MembershipViewModel(WebService webService)
    {
        this.webService = webService;

    }

    public int MembershipId
    {
        get { return _membershipId; }
        set
        {
            _membershipId = value;
            Debug.WriteLine($"MembershipId set to {_membershipId}");
            OnPropertyChanged(nameof(MembershipId));

            // Load the Membership when the MembershipId is set
            LoadMembership();
        }
    }

    private async Task LoadMembership()
    {
        try
        {
            Membership = await webService.GetMembershipById(MembershipId);
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