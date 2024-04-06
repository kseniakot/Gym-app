using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
using Gym.View;
using System.Windows.Input;
namespace Gym.ViewModel;

    public partial class SignInViewModel : ObservableObject
    {
    readonly DataBaseService _dbService;


    [ObservableProperty]
    private User _user = new();

    public SignInViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task SignInAsync()
    {

        if (string.IsNullOrWhiteSpace(User.Email) || string.IsNullOrWhiteSpace(User.Password))
        {
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (!_dbService.IsUserExist(User.Email))
        {
            await Shell.Current.DisplayAlert("This e-mail is not registered", "Please try another one.", "Ok");
        }
        else if (_dbService.IsBannedByEmail(User.Email))
        {
            User = new();
            await Shell.Current.DisplayAlert("This user is banned", "No access", "Ok");
        }
        else
        {
            if (User.Email == "admin" && User.Password == "admin")
            {
                //await Shell.Current.GoToAsync("//AdminMainPage");
                Application.Current.MainPage = new AdminShell();
            }
            else
            {
                //await Shell.Current.GoToAsync("//UserMainPage");
                Application.Current.MainPage = new UserShell();
            }
        }
    }   


}

