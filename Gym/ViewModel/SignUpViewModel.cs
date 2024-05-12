using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
using System.Text.RegularExpressions;
using Gym.Exceptions;
namespace Gym.ViewModel;

public partial class SignUpViewModel : ObservableObject
{
      WebService webService;

    [ObservableProperty]
    private User _user = new();

    [ObservableProperty]
    private string _phoneNumber = "";
    //PASSWORD VISIBILITY
    [ObservableProperty]
    bool _isPasswordHidden = true;
    [ObservableProperty]
    string _imageSource = "eyeopen.png";

    public SignUpViewModel(WebService webService)
	{
		this.webService = webService;
	}

    private static bool IsAnyNullOrEmpty(object user)
    {
        return user.GetType()
            .GetProperties()
            .Where(pt => pt.PropertyType == typeof(string))
            .Select(v => (string)v.GetValue(user))
            .Any(value => string.IsNullOrWhiteSpace(value));
    }


    private async Task<bool> IsUserExistAsync()
    {
        try
        {
            return await webService.IsUserExistAsync(User.Email);
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

        else if (!Regex.IsMatch(User.PhoneNumber, @"^\+375\(\d{2}\)\d{3}-\d{2}-\d{2}"))
        {
            // User = new();
            await Shell.Current.DisplayAlert("Invalid phone number", "Enter phone number in +375(XX)XXX-XX-XX format.", "Ok");
        }
        else if (await IsUserExistAsync())
        {
            // User = new();
            await Shell.Current.DisplayAlert("This e-mail is already in use", "Please try another one.", "Ok");
        }
        else
        {
            try
            {
                await webService.AddUserAsync(User);
                User = new();
                PhoneNumber = "";
                await Shell.Current.DisplayAlert("User added successfully", "Press Ok and sign in", "Ok");
                await Shell.Current.GoToAsync("//SignIn");
            }
            catch (SessionExpiredException)
            {
                await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
                await Shell.Current.GoToAsync("SignInView");
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Something went wrong", "Please try again", "Ok");
            }
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