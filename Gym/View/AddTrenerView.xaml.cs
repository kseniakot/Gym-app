using Gym.ViewModel;
namespace Gym.View;

public partial class AddTrenerView : ContentPage
{
	public AddTrenerView(AddTrenerViewModel addTrenerViewModel)
	{
		InitializeComponent();
		BindingContext = addTrenerViewModel;
	}
}