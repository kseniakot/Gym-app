using Gym.ViewModel;
namespace Gym.View;

public partial class FreezeListView : ContentPage
{
    readonly FreezeListViewModel _FreezeListViewModel;

    public FreezeListView(FreezeListViewModel FreezeListViewModel)
    {
        _FreezeListViewModel = FreezeListViewModel;
        InitializeComponent();
        BindingContext = _FreezeListViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _FreezeListViewModel.LoadDataCommand.Execute(null);
    }
}