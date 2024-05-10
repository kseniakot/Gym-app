using Gym.ViewModel;
namespace Gym.View;

public partial class ProfileView : ContentPage
{
    ProfileViewModel _profileViewModel;
	public ProfileView(ProfileViewModel profileViewModel)
	{
        _profileViewModel = profileViewModel;
		InitializeComponent();
		BindingContext = profileViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _profileViewModel.LoadDataCommand.Execute(null);
    }
}