using Gym.ViewModel;
namespace Gym.View;

public partial class AddMembershipView : ContentPage
{
	public AddMembershipView(AddMembershipViewModel addMembershipViewModel)
	{
		InitializeComponent();
		BindingContext = addMembershipViewModel;
	}
}