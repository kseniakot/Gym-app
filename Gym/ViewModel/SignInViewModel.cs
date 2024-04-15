using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
using Gym.View;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Gym.ViewModel;

public partial class SignInViewModel : ObservableObject
{

    // readonly DataBaseService _dbService;
    readonly WebService _webService;


    [ObservableProperty]
    private User _user = new();

    //show password
    [ObservableProperty]
    bool _isPasswordHidden = true;
    [ObservableProperty]
    string _imageSource = "eyeopen.png";

    //public SignInViewModel(DataBaseService dbService)
    //{
    //    _dbService = dbService;
    //}

    public SignInViewModel(WebService webService)
    {
        _webService = webService;
    }

    [RelayCommand]
    private void ShowPassword()
    {
        if (!IsPasswordHidden)
        {
            ImageSource = "eyeopen.png";

        }
        else
        {
            ImageSource = "eyeclose.png";
        }
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private async Task SignInAsync()
    {

        if (string.IsNullOrWhiteSpace(User.Email) || string.IsNullOrWhiteSpace(User.Password))
        {
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (!Regex.IsMatch(User.Email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") && User.Email != "admin")
        {
            await Shell.Current.DisplayAlert("Invalid e-mail", "Please try again.", "Ok");
        }

        else
        {
            try
            {
                await _webService.LogIn(User);

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
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }


        }



    }
}




