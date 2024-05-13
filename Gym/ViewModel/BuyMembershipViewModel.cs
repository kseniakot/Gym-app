using CommunityToolkit.Mvvm.ComponentModel;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using CommunityToolkit.Mvvm.Input;

namespace Gym.ViewModel;

public partial class BuyMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership? _membership;
    [ObservableProperty]
    private string _promoCode;
    private int _membershipId;
    readonly WebService webService;

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


    public BuyMembershipViewModel(WebService webService)
    {
        this.webService = webService;

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

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("//ShopView");
    }


    [RelayCommand]
    private async Task PayAsync()
    {
        try
        {
          //  await webService.BuyMembership(Membership);
          await webService.MakePayment(Membership.Id);

           // await Shell.Current.DisplayAlert("Success", "Check membership in your profile", "Ok");
           // await CancelAsync();
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