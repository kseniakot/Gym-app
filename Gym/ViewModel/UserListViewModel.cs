using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using System.Runtime.InteropServices;

namespace Gym.ViewModel;

public partial class UserListViewModel : ObservableObject
{
    [ObservableProperty]
    private User _selectedUser;
    [ObservableProperty]
    private ObservableCollection<User> _users;

    [ObservableProperty]
    private string _searchText;

    readonly DataBaseService _dbService;
    public UserListViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
        Users = new ObservableCollection<User>(dbService.GetUnbannedUsers());
    }
    [RelayCommand]
    private async Task RemoveUserAsync()
    {
        if (SelectedUser != null)
        {
            //  await DatabaseService<User>.RemoveColumnAsync(SelectedUser.Id);
            _dbService.DeleteUser(SelectedUser);
            Users.Remove(SelectedUser);

            SelectedUser = null;
        }
        else
        {
            await Shell.Current.DisplayAlert("No user selected", "Please select and try again.", "Ok");
        }
    }


    [RelayCommand]
    private async Task AddUserAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddUserView));
    }

    [RelayCommand]
    private async Task BunUser()
    {
            _dbService.BanUser(SelectedUser);
        Users.Remove(SelectedUser);
            await Shell.Current.DisplayAlert("SUCCESS", "The user was banned successfully", "OK");
        
    }
    [RelayCommand]
    private void LoadData()
    {
        Users = new ObservableCollection<User>(_dbService.GetUnbannedUsers());
    }

    [RelayCommand]
    private void SearchUser()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Users = new ObservableCollection<User>(_dbService.GetUnbannedUsers());
        }
        else
        {
            Users = new ObservableCollection<User>(_dbService.GetUnbannedUsers().Where(user => user.Name.Contains(SearchText)));
        }
    }

    [RelayCommand]
    private async Task TapBanned()
    {
        await Shell.Current.GoToAsync(nameof(BannedListView));
    }

}
