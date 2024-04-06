using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;

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
        Users = new ObservableCollection<User>(dbService.GetAllUsers());
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
        //if (dbService.IsUserBannedById(_selectedUser.Id))
        //{
        //    await dbService.UnbanUser(_selectedUser.Id);
        //    await Shell.Current.DisplayAlert("Title", "UNBAN", "ok");
        //}
        //else
        //{
        //    await dbService.BanUser(_selectedUser.Id);
        //    await Shell.Current.DisplayAlert("Title", "BAN", "ok");
        //}
    }
    [RelayCommand]
    private void LoadData()
    {
        Users = new ObservableCollection<User>(_dbService.GetAllUsers());
    }

}
