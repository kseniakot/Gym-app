using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
namespace Gym.ViewModel;

public partial class EditMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership? _membership;
    [ObservableProperty]
    private ObservableCollection<Freeze> _freezes;
    // [ObservableProperty]
    private Freeze _selectedFreeze;

    public Freeze SelectedFreeze
    {
        get { return _selectedFreeze; }
        set
        {
            _selectedFreeze = value;
            //Membership.Freeze = value;
            Membership.FreezeId = value.Id;

            //Debug.WriteLine(Membership.Freeze.Id);
        }
    }

    private int _membershipId;
    readonly WebService webService;
    public EditMembershipViewModel(WebService webService)
    {
       this.webService = webService;
        InitializeAsync();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Freezes = new ObservableCollection<Freeze>(await webService.GetAllFreezes());
           
            //Freezes.Insert(0, new Freeze { Id = 0, Name = "None" });
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

    private static bool IsAnyNullOrEmpty(object membership)
    {
        var propertiesToCheck = new[] { "Name", "Months", "Price" };

        return membership.GetType()
            .GetProperties()
            .Where(pt => propertiesToCheck.Contains(pt.Name) && (pt.PropertyType == typeof(string) || pt.PropertyType == typeof(int?) || pt.PropertyType == typeof(decimal?)))
            .Select(v => v.GetValue(membership))
            .Any(value => value == null || string.IsNullOrWhiteSpace(value.ToString()));
    }

    private async Task<bool> DoesMembershipExist(Membership membership) 
    {
        try
        {
            return await webService.DoesMembershipExistAsync(membership);
        }
        catch (SessionExpiredException)
        {
            await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
            await Shell.Current.GoToAsync("SignInView");
            Application.Current.MainPage = new AppShell();
            return false;

        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Something went wrong", "Please try again", "Ok");
            return false;
        }
    }


    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

   

    [RelayCommand]
    private async Task EditAsync()
    {
        if (IsAnyNullOrEmpty(Membership))
        {
            Membership = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (await DoesMembershipExist(Membership))
        {
           // Membership = new();
            await Shell.Current.DisplayAlert("Such membership already exists", "Make some changes or go back", "Ok");
        }
        else
        {
            try
            {
                await webService.EditMembership(Membership);
                await Shell.Current.DisplayAlert("Memebrship edited successfully", "Press Ok and return to the membership list", "Ok");

                BackCommand.Execute(null);
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