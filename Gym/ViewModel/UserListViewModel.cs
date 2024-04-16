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

    readonly WebService webService;
    public UserListViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        Users = new ObservableCollection<User>(await webService.GetUnbannedUsers());
    }


    [RelayCommand]
    private async Task RemoveUserAsync()
    {
        if (SelectedUser != null)
        {
            try
            {
                await webService.RemoveUser(SelectedUser.Id);
                Users.Remove(SelectedUser);
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");

            }
            finally
            {
                SelectedUser = null;
            }
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
    private async Task BunUserAsync()
    {
        try
        {
            webService.BanUser(SelectedUser.Id);
            Users.Remove(SelectedUser);
            await Shell.Current.DisplayAlert("SUCCESS", "The user was banned successfully", "OK");
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
        }
        finally
        {
            SelectedUser = null;
        }
        
    }


    [RelayCommand]
    private async Task LoadData()
    {
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task SearchUserAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Users = new ObservableCollection<User>(await webService.GetUnbannedUsers());
        }
        else
        {
            Users = new ObservableCollection<User>((await webService.GetUnbannedUsers()).Where(user => user.Name.Contains(SearchText)));
        }
    }

    [RelayCommand]
    private async Task TapBanned()
    {
        await Shell.Current.GoToAsync(nameof(BannedListView));
    }

}
