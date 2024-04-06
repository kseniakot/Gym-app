using Gym.ViewModel;
namespace Gym.View;

public partial class BannedListView : ContentPage
{
    readonly BannedListViewModel _bannedListViewModel;
    public BannedListView(BannedListViewModel bannedListViewModel)
	{
		_bannedListViewModel = bannedListViewModel;
		InitializeComponent();
		BindingContext = _bannedListViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _bannedListViewModel.LoadDataCommand.Execute(null);
    }
}