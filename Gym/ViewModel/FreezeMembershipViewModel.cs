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

    [ObservableProperty]
    private DateTime _selectedDateFrom = DateTime.Today.ToUniversalTime();
    [ObservableProperty]
    private DateTime _selectedDateTo = DateTime.Today.ToUniversalTime();

    [ObservableProperty]
    bool _isDatePickerVisible = true;
    [ObservableProperty]
    int _FreezeDaysLeft;

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
            FreezeDaysLeft = Membership.ActiveFreeze.DaysLeft;
            MaximumDate = DateTime.Today.AddDays(FreezeDaysLeft);
            if (MaximumDate == DateTime.Today)
            {
                IsDatePickerVisible = false;
            }
            
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
        if (!IsDatePickerVisible)
        {
            await Shell.Current.DisplayAlert("Error", "You used all your freeze days :(", "Ok");
        }
        else
        {
            int MinDays = Membership.Membership.Freeze.MinDays.Value;
            if ((SelectedDateTo - SelectedDateFrom).Days < MinDays)
            {
                await Shell.Current.DisplayAlert("Error", "You cannot freeze membership for less than " + MinDays + " days", "Ok");
            }
            else
            {
                try
                {
                    Membership.ActiveFreeze.StartDate = SelectedDateFrom.ToUniversalTime(); ;
                    Membership.ActiveFreeze.EndDate = SelectedDateTo.ToUniversalTime();

                    Membership.ActiveFreeze.DaysLeft -= (SelectedDateTo - SelectedDateFrom).Days;
                    Membership.Status = Status.Frozen;
                    Membership.EndDate += (SelectedDateTo.ToUniversalTime() - SelectedDateFrom.ToUniversalTime());





                    await webService.FreezeMembership(Membership);
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
    }


}