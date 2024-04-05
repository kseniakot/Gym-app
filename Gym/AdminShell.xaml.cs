using Gym.View;
namespace Gym;

public partial class AdminShell : Shell
{
	public AdminShell()
	{
        InitializeComponent();
        Routing.RegisterRoute("SignInPage", typeof(SignInView));
       // Routing.RegisterRoute("AddUserView", typeof(AddUserView));
       // Routing.RegisterRoute("AddBookView", typeof(AddBookView));
    }

    private async void LogOutAdmin_Button(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync("SignInPage");
        Application.Current.MainPage = new AppShell();

    }
}