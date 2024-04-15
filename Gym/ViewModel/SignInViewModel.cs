﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
using Gym.View;
using System.Windows.Input;
using System.Text.RegularExpressions;
namespace Gym.ViewModel;

    public partial class SignInViewModel : ObservableObject
    {
    readonly DataBaseService _dbService;


    [ObservableProperty]
    private User _user = new();

    //show password
    [ObservableProperty]
    bool _isPasswordHidden = true;
    [ObservableProperty]
    string _imageSource = "eyeopen.png";

    public SignInViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
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
                if (!_dbService.IsPasswordCorrect(User.Email, User.Password))
                {
                    User.Password = ""; 
                    await Shell.Current.DisplayAlert("Password is incorrect", "Please try again.", "Ok");
                }
                else { 
                //await Shell.Current.GoToAsync("//UserMainPage");
                Application.Current.MainPage = new UserShell();
                 }
            }
        }
    }   


}

