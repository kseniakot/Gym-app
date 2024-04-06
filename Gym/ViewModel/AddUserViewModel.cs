using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
namespace Gym.ViewModel;

public partial class AddUserViewModel : ObservableObject
{

    [ObservableProperty]
    private User _user = new();
    readonly DataBaseService _dbService;
    public AddUserViewModel(DataBaseService dbService)
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
        return _dbService.IsUserExist(User.Email);
    }

    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task AddAsync()
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
            await Shell.Current.DisplayAlert("User added successfully", "Press Ok and return to the user list", "Ok");

            BackCommand.Execute(null);
        }
    }


}