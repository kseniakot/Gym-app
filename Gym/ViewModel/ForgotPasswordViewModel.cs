
//using Android.OS;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        try
        {
            // Shell.Current.DisplayAlert("Alert", "Go to this link to reset password: <a href='https://", "ok");
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(Email, "Reset Password",
                "Go to this link to reset password: <a href='http://192.168.56.1:5119/users/resetpassword/'>link</a>");
        }
        catch (System.IO.IOException ex)
        {
            Shell.Current.DisplayAlert("Alert", $"{ex.Message}", "ok");
        }
       
    }
}