using Gym.View;
using Gym.ViewModel;

namespace Gym.View;

public partial class SignInView : ContentPage
{
    readonly SignInViewModel _signInViewModel;

    public SignInView(SignInViewModel signInViewModel)
	{
        _signInViewModel = signInViewModel;
		InitializeComponent();
        BindingContext = signInViewModel;
    }

    private async void TapGestureRecognizer_Tapped_For_SignUP(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignUp");
    }
}