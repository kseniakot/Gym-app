using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.Model;
using Gym.Services;
namespace Gym.ViewModel;

public partial class SignUpViewModel : ObservableObject
{
      DataBaseService _dbService;

    [ObservableProperty]
    private User _user = new();
    public SignUpViewModel(DataBaseService dbService)
	{
		_dbService = dbService;
	}

    private static bool IsAnyNullOrEmpty(object user)
    {
        return user.GetType()
            .GetProperties()
            .Where(pt => pt.PropertyType == typeof(string))
            .Select(v => (string)v.GetValue(user))
            .Any(value => string.IsNullOrWhiteSpace(value));
    }

    private bool IsUserExist()
    {
        return _dbService.IsUserExist(_user.Email);
    }


    [RelayCommand]
	private async Task SignUpAsync()
	{
        if (IsAnyNullOrEmpty(User))
        {
            User = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (IsUserExist())
        {
            User = new();
            await Shell.Current.DisplayAlert("This e-mail is already in use", "Please try another one.", "Ok");
        }
        else
        {
            //await DatabaseService<User>.AddColumnAsync(User);
            _dbService.AddUser(User);
            await Shell.Current.DisplayAlert("User added successfully", "Press Ok and sign in", "Ok");
            await Shell.Current.GoToAsync("//SignIn");
        }

	}
}