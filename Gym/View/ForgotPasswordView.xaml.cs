using Gym.ViewModel;
namespace Gym.View;

public partial class ForgotPasswordView : ContentPage
{
	public ForgotPasswordView(ForgotPasswordViewModel forgotPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = forgotPasswordViewModel;
	}
}