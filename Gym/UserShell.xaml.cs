namespace Gym.View;
//namespace Gym;

public partial class UserShell : Shell
{
	public UserShell()
	{
        Routing.RegisterRoute("SignInPage", typeof(SignInView));
        InitializeComponent();
    }
}