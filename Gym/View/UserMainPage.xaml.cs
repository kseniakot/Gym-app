using Gym.ViewModel;
namespace Gym.View;

public partial class UserMainPage : ContentPage
{
    UserMainViewModel _userMainViewModel;
	public UserMainPage(UserMainViewModel userMainViewModel)
	{
		InitializeComponent();
		BindingContext = userMainViewModel;
        _userMainViewModel = userMainViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _userMainViewModel.LoadDataCommand.Execute(null);
    }
}