using Gym.ViewModel;
namespace Gym.View;

public partial class AddUserView : ContentPage
{
	public AddUserView(AddUserViewModel addUserViewModel)
	{
		InitializeComponent();
		BindingContext = addUserViewModel;
	}
}