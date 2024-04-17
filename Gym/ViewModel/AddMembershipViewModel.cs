using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
namespace Gym.ViewModel;

public partial class AddMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership _membership = new();
    readonly WebService webService;
    public AddMembershipViewModel(WebService webService)
    {
        this.webService = webService;
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
    private async Task AddAsync()
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
                await webService.AddMembership(Membership);
                await Shell.Current.DisplayAlert("Memebrship added successfully", "Press Ok and return to the membership list", "Ok");

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
