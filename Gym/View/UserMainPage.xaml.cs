using Gym.ViewModel;
namespace Gym.View;

public partial class UserMainPage : ContentPage
{
	public UserMainPage(UserMainViewModel userMainViewModel)
	{
		InitializeComponent();
		BindingContext = userMainViewModel;
	}
}