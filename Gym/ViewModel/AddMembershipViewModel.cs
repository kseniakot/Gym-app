using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
namespace Gym.ViewModel;

public partial class AddMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership _membership = new();
    readonly DataBaseService _dbService;
    public AddMembershipViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
    }

    private static bool IsAnyNullOrEmpty(object membership)
    {
        return membership.GetType()
            .GetProperties()
            .Where(pt => pt.PropertyType == typeof(string))
            .Select(v => (string)v.GetValue(membership))
            .Any(value => string.IsNullOrWhiteSpace(value));
    }

    private bool IsMembershipExist()
    {
        return _dbService.IsMembershipExist(Membership);
    }

    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (IsAnyNullOrEmpty(Membership))
        {
            Membership = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (IsMembershipExist())
        {
            Membership = new();
            await Shell.Current.DisplayAlert("This membership already exist", "Please try another one.", "Ok");
        }
        else
        {
            //await DatabaseService<User>.AddColumnAsync(User);
            _dbService.AddMembership(Membership);
            await Shell.Current.DisplayAlert("Memebrship added successfully", "Press Ok and return to the membership list", "Ok");

            BackCommand.Execute(null);
        }
    }
}