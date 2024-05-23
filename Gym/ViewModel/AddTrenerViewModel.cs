using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
using System.Text.RegularExpressions;
namespace Gym.ViewModel;

public partial class AddTrenerViewModel : ObservableObject
{

    [ObservableProperty]
    private Trener _trener = new();

    [ObservableProperty]
    private string _phoneNumber = "";
    //PASSWORD VISIBILITY
    [ObservableProperty]
    bool _isPasswordHidden = true;
    [ObservableProperty]
    string _imageSource = "eyeopen.png";

    readonly WebService webService;


    public AddTrenerViewModel(WebService webService)
    {
        this.webService = webService;
    }

    private static bool IsAnyNullOrEmpty(object Trener)
    {
        var propertiesToCheck = new[] { "Name", "PhoneNumber", "Email", "Password" };

        return Trener.GetType()
            .GetProperties()
            .Where(pt => propertiesToCheck.Contains(pt.Name) && (pt.PropertyType == typeof(string)))
            .Select(v => v.GetValue(Trener))
            .Any(value => value == null || string.IsNullOrWhiteSpace(value.ToString()));
    }


    private async Task<bool> IsTrenerExistAsync()
    {
        try
        {
            return await webService.IsUserExistAsync(Trener.Email);
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
        PhoneNumber = FormatPhoneNumber(PhoneNumber);
        Trener.PhoneNumber = PhoneNumber;

        if (IsAnyNullOrEmpty(Trener))
        {
            // Trener = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (!Regex.IsMatch(Trener.Email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
        {
            await Shell.Current.DisplayAlert("Invalid e-mail", "Please try again.", "Ok");
        }
        else if (!Regex.IsMatch(Trener.PhoneNumber, @"^\+375\(\d{2}\)\d{3}-\d{2}-\d{2}"))
        {
            // Trener = new();
            await Shell.Current.DisplayAlert("Invalid phone number", "Enter phone number in +375(XX)XXX-XX-XX format.", "Ok");
        }
        else if (await IsTrenerExistAsync())
        {
            // Trener = new();
            await Shell.Current.DisplayAlert("This e-mail is already in use", "Please try another one.", "Ok");
        }
        else
        {
            try
            {
                await webService.AddTrener(Trener);
                await Shell.Current.DisplayAlert("Trener added successfully", "Press Ok and return to the Trener list", "Ok");
                BackCommand.Execute(null);
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