using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
namespace Gym.ViewModel;

public partial class BannedListViewModel : ObservableObject
{
    [ObservableProperty]
    private User _selectedUser;
    [ObservableProperty]
    private ObservableCollection<User> _users;

    [ObservableProperty]
    private string _searchText;

    readonly DataBaseService _dbService;
    public BannedListViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
        Users = new ObservableCollection<User>(_dbService.GetBannedUsers());
    }
    
    [RelayCommand]
    private async Task UnbanUser()
    {
        _dbService.UnbanUser(SelectedUser);
        Users.Remove(SelectedUser);
        await Shell.Current.DisplayAlert("SUCCESS", "The user was unbanned successfully", "OK");

    }
    [RelayCommand]
    private void LoadData()
    {
        Users = new ObservableCollection<User>(_dbService.GetBannedUsers());
    }

    [RelayCommand]
    private void SearchUser()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Users = new ObservableCollection<User>(_dbService.GetBannedUsers());
        }
        else
        {
            Users = new ObservableCollection<User>(_dbService.GetBannedUsers().Where(user => user.Name.Contains(SearchText)));
        }
    }

    [RelayCommand]
    private async Task TapBanned()
    {
        await Shell.Current.GoToAsync(nameof(BannedListView));
    }

}