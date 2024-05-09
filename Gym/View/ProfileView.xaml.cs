using Gym.ViewModel;
namespace Gym.View;

public partial class ProfileView : ContentPage
{
	public ProfileView(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
	}
}