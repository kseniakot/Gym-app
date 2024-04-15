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
    [ObservableProperty]
    private string _phoneNumber = "";
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
        PhoneNumber = FormatPhoneNumber(PhoneNumber);
        User.PhoneNumber = PhoneNumber;
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
        else if (!Regex.IsMatch(User.PhoneNumber, @"^\+375\(\d{2}\)\d{3}-\d{2}-\d{2}"))
        {
           // User = new();
            await Shell.Current.DisplayAlert("Invalid phone number", "Enter phone number in +375(XX)XXX-XX-XX format.", "Ok");
        }
        else
        {
            //await DatabaseService<User>.AddColumnAsync(User);
            _dbService.AddUser(User);
            User = new();
            PhoneNumber = "";
            await Shell.Current.DisplayAlert("User added successfully", "Press Ok and sign in", "Ok");
            await Shell.Current.GoToAsync("//SignIn");
        }

	}

    private string FormatPhoneNumber(string rawPhoneNumber)
    {
        // Remove any non-digit characters
        var digits = new string(rawPhoneNumber.Where(char.IsDigit).ToArray());

        // Insert the formatting characters at the correct positions
        if (digits.Length > 3) digits = digits.Insert(3, "(");
        if (digits.Length > 6) digits = digits.Insert(6, ")");
        
        if (digits.Length > 11) digits = digits.Insert(10, "-");
        if (digits.Length > 14) digits = digits.Insert(13, "-");

        return "+" + digits;
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


}