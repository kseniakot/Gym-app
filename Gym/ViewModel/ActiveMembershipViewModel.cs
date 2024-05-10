namespace Gym.ViewModel;

public class ActiveMembershipViewModel : ContentPage
{
	public ActiveMembershipViewModel()
	{
		Content = new StackLayout
		{
			Children = {
				new Label { Text = "Welcome to .NET MAUI!" }
			}
		};
	}
}