using Gym.View;
namespace Gym;

public partial class AdminShell : Shell
{
	public AdminShell()
	{
        InitializeComponent();
        Routing.RegisterRoute("SignInPage", typeof(SignInView));
        Routing.RegisterRoute("AddUserView", typeof(AddUserView));
        Routing.RegisterRoute("BannedListView", typeof(BannedListView));
        Routing.RegisterRoute("AddMembershipView", typeof(AddMembershipView));
        Routing.RegisterRoute("EditMembershipView", typeof(EditMembershipView));
    }

    private async void LogOutAdmin_Button(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync("SignInPage");
        Application.Current.MainPage = new AppShell();

    }
}