using Gym.ViewModel;
using Gym.View;
//using Android.App.Job;
namespace Gym.View;

public partial class SignUpView : ContentPage
{

    readonly SignUpViewModel _signUpViewModel;
    public SignUpView(SignUpViewModel signUpViewModel)
    {
        InitializeComponent();
        _signUpViewModel = signUpViewModel;
        BindingContext = signUpViewModel;
    }

    private async void TapGestureRecognizer_Tapped_For_SignIn(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignIn");
    }

}