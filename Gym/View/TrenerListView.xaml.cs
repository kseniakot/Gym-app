using Gym.ViewModel;
namespace Gym.View;

public partial class TrenerListView : ContentPage
{
    public TrenerListView(TrenerListViewModel trenerListViewModel)
    {
        InitializeComponent();
        BindingContext = trenerListViewModel;
    }
}