using Gym.ViewModel;
namespace Gym.View;

public partial class AddFreezeView : ContentPage
{
    public AddFreezeView(AddFreezeViewModel addFreezeViewModel)
    {
        InitializeComponent();
        BindingContext = addFreezeViewModel;
    }
}