using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
///using Org.Apache.Http.Authentication;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class SelectTimeViewModel : ObservableObject
{
    [ObservableProperty]
    private TimeSpan _selectedTime = TimeSpan.MinValue;


    [ObservableProperty]
    DateTime _date;


     readonly WebService webService;
    public SelectTimeViewModel(WebService webService)
    {
        this.webService = webService;
    }

  

    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task AddAsync()
    {

            try
            {
                await webService.AddWorkHour((await webService.GetUserFromToken()).Id, Date, SelectedTime);
                await Shell.Current.DisplayAlert("New time added successfully", "Press Ok and return to your schedule", "Ok");

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
