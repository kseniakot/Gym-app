using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
using System.Text.RegularExpressions;
namespace Gym.ViewModel;

public partial class SignUpViewModel : ObservableObject
{
      DataBaseService _dbService;

    [ObservableProperty]
    private User _user = new();
    //[ObservableProperty]
    //private string _phoneNumber = "";
    //show password
    [ObservableProperty]
    bool _isPasswordHidden = true;
    [ObservableProperty]
    string _imageSource = "eyeopen.png";

    public SignUpViewModel(DataBaseService dbService)
	{
		_dbService = dbService;
	}

    private static bool IsAnyNullOrEmpty(object user)
    {
        return user.GetType()
            .GetProperties()
            .Where(pt => pt.PropertyType == typeof(string))
            .Select(v => (string)v.GetValue(user))
            .Any(value => string.IsNullOrWhiteSpace(value));
    }

    private bool IsUserExist()
    {
        return _dbService.IsUserExist(_user.Email);
    }


    [RelayCommand]
	private async Task SignUpAsync()
	{
      //  User.PhoneNumber = PhoneNumber;
        if (IsAnyNullOrEmpty(User))
        {
           // User = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (!Regex.IsMatch(User.Email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
        {
            await Shell.Current.DisplayAlert("Invalid e-mail", "Please try again.", "Ok");
        }
        else if (IsUserExist())
        {
           // User = new();
            await Shell.Current.DisplayAlert("This e-mail is already in use", "Please try another one.", "Ok");
        }
        else if (!Regex.IsMatch(User.PhoneNumber, @"^\+375\(\d{2}\)\d{7}$"))
        {
           // User = new();
            await Shell.Current.DisplayAlert("Invalid phone number", "Enter phone number in +375(XX)XXXXXXX format.", "Ok");
        }
        else
        {
            //await DatabaseService<User>.AddColumnAsync(User);
            _dbService.AddUser(User);
            User = new();
            await Shell.Current.DisplayAlert("User added successfully", "Press Ok and sign in", "Ok");
            await Shell.Current.GoToAsync("//SignIn");
        }

	}

    //[RelayCommand]

    //private void TextChanged()
    //{
    //    if (PhoneNumber.Length == 4)
    //    {
    //        PhoneNumber += "(";
    //    }
    //    if (PhoneNumber.Length == 9)
    //    {
    //        PhoneNumber += ")";
    //    }
    //    if (PhoneNumber.Length == 10)
    //    {
    //        PhoneNumber += " ";
    //    }
    //    if (PhoneNumber.Length == 13)
    //    {
    //        PhoneNumber += "-";
    //    }
    //    if (PhoneNumber.Length == 14)
    //    {
    //        PhoneNumber += "-";
    //    }

    //}

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


}