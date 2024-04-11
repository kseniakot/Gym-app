using Gym.ViewModel;
namespace Gym.View;

public partial class ShopView : ContentPage
{
	public ShopView(ShopViewModel shopViewModel)
	{
		InitializeComponent();
		BindingContext = shopViewModel;
	}
}