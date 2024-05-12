
//using Android.OS;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using Gym.Model;
using Gym.Services;
 

namespace Gym.ViewModel;

public partial class ForgotPasswordViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email;

    readonly WebService _webService;
    public ForgotPasswordViewModel(WebService webService)
	{
		_webService = webService;
    }

    [RelayCommand]
    public async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (!Regex.IsMatch(Email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
        {
            await Shell.Current.DisplayAlert("Invalid e-mail", "Please try again.", "Ok");
        }
        else
        {
            try
            {
                await _webService.ResetPassword(Email);
                await Shell.Current.DisplayAlert("Success", "Password reset link has been sent to your email", "Ok");
                await Shell.Current.GoToAsync("//SignIn");

                Email = "";
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }
       
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("//SignIn");
        Email="";
    }
}