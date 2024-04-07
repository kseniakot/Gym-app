using Gym.ViewModel;
namespace Gym.View;

public partial class UserListView : ContentPage
{
    readonly UserListViewModel _userListViewModel;
  
    public UserListView(UserListViewModel userListViewModel)
	{
        _userListViewModel = userListViewModel;
		InitializeComponent();
        BindingContext = _userListViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _userListViewModel.LoadDataCommand.Execute(null);
    }
}