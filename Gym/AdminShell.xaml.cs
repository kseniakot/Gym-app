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
}