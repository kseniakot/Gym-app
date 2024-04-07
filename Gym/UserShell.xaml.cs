namespace Gym.View;
//namespace Gym;

public partial class UserShell : Shell
{
	public UserShell()
	{
        Routing.RegisterRoute("SignInPage", typeof(SignInView));
        InitializeComponent();
    }

    private async void LogOut_Button(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SignInPage");
        Application.Current.MainPage = new AppShell();
    }

}