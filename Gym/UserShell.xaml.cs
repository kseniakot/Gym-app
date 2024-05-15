namespace Gym.View;
//namespace Gym;

public partial class UserShell : Shell
{
	public UserShell()
	{
        Routing.RegisterRoute("SignInView", typeof(SignInView));
        Routing.RegisterRoute("ShopView", typeof(ShopView));
        Routing.RegisterRoute("MembershipView", typeof(MembershipView));
        Routing.RegisterRoute("BuyMembershipView", typeof(BuyMembershipView));
        Routing.RegisterRoute("ProfileView", typeof(ProfileView));
        Routing.RegisterRoute("ActivateMembershipView", typeof(ActivateMembershipView));
        Routing.RegisterRoute("FreezeMembershipView", typeof(FreezeMembershipView));
        Routing.RegisterRoute("CancelFreezeView", typeof(CancelFreezeView));
        Routing.RegisterRoute("TestPayment", typeof(TestPayment));
        InitializeComponent();
    }

    private async void LogOut_Button(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SignInView");
        Application.Current.MainPage = new AppShell();
    }

}