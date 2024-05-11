using Gym.View;
namespace Gym;

public partial class AdminShell : Shell
{
	public AdminShell()
	{
        InitializeComponent();
        Routing.RegisterRoute("SignInView", typeof(SignInView));
      //  Routing.RegisterRoute("SignUpPage", typeof(SignUpView));
        Routing.RegisterRoute("AddUserView", typeof(AddUserView));
        Routing.RegisterRoute("BannedListView", typeof(BannedListView));
        Routing.RegisterRoute("AddMembershipView", typeof(AddMembershipView));
        Routing.RegisterRoute("AddFreezeView", typeof(AddFreezeView));
        Routing.RegisterRoute("EditMembershipView", typeof(EditMembershipView));
        Routing.RegisterRoute("EditFreezeView", typeof(EditFreezeView));

    }

    private async void LogOutAdmin_Button(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync("SignInView");
        Application.Current.MainPage = new AppShell();

    }
}