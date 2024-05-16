using CommunityToolkit.Mvvm.ComponentModel;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

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

    //public event Action<string> LoadPaymentPageRequested;
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
        string url = await webService.MakePayment((await webService.GetUserFromToken()).Id, Membership);
        string encodedUrl = Uri.EscapeDataString(url);
        await Shell.Current.GoToAsync($"TestPayment?Url={encodedUrl}");
        Debug.WriteLine(url);
        //LoadPaymentPageRequested?.Invoke(url);
    }

}