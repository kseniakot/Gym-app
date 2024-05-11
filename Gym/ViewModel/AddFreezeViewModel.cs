using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
///using Org.Apache.Http.Authentication;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class AddFreezeViewModel : ObservableObject
{
    [ObservableProperty]
    private Freeze _Freeze = new();
    [ObservableProperty]

    private ObservableCollection<Freeze> _freezes;
    // [ObservableProperty]
    //private Freeze _selectedFreeze;

   
    readonly WebService webService;
    public AddFreezeViewModel(WebService webService)
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

    private static bool IsAnyNullOrEmpty(object Freeze)
    {
        var propertiesToCheck = new[] { "Name", "Months", "Price" };

        return Freeze.GetType()
            .GetProperties()
            .Where(pt => propertiesToCheck.Contains(pt.Name) && (pt.PropertyType == typeof(string) || pt.PropertyType == typeof(int?) || pt.PropertyType == typeof(decimal?)))
            .Select(v => v.GetValue(Freeze))
            .Any(value => value == null || string.IsNullOrWhiteSpace(value.ToString()));
    }

    private async Task<bool> DoesFreezeExist(Freeze Freeze)
    {
        try
        {
            return await webService.DoesFreezeExistAsync(Freeze);
        }
        catch (SessionExpiredException)
        {
            await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
            await Shell.Current.GoToAsync("SignInView");
            Application.Current.MainPage = new AppShell();
            return false;

        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Something went wrong", e.Message, "Ok");
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

        if (IsAnyNullOrEmpty(Freeze))
        {
            Freeze = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (await DoesFreezeExist(Freeze))
        {
            // Freeze = new();
            await Shell.Current.DisplayAlert("Such Freeze already exists", "Make some changes or go back", "Ok");
        }
        else
        {
            try
            {
                await webService.AddFreeze(Freeze);
                await Shell.Current.DisplayAlert("Freeze added successfully", "Press Ok and return to the Freeze list", "Ok");

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
