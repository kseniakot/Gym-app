using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using System.Runtime.InteropServices;
using Gym.Exceptions;

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
        try { 
        Users = new ObservableCollection<User>(await webService.GetUnbannedUsers());
    } catch (SessionExpiredException)
        {
        await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
        await Shell.Current.GoToAsync("SignInView");
        Application.Current.MainPage = new AppShell();
    }
    catch (Exception e)
        {
        await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
    }
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
            catch (SessionExpiredException)
            {
                await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
                await Shell.Current.GoToAsync("SignInView");
                Application.Current.MainPage = new AppShell();
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
        if (SelectedUser == null)
        {
            await Shell.Current.DisplayAlert("No user selected", "Please select and try again.", "Ok");
            return;
        }
        else
        {
            try
            {
                webService.BanUser(SelectedUser.Id);
                Users.Remove(SelectedUser);
                await Shell.Current.DisplayAlert("SUCCESS", "The user was banned successfully", "OK");
            }
            catch (SessionExpiredException)
            {
                await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
                await Shell.Current.GoToAsync("SignInView");
                Application.Current.MainPage = new AppShell();
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
            try
            {
                Users = new ObservableCollection<User>(await webService.GetUnbannedUsers());
            } catch (SessionExpiredException)
            {
                   await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
                await Shell.Current.GoToAsync("SignInView");
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }
        else
        {
            try
            {
                Users = new ObservableCollection<User>((await webService.GetUnbannedUsers()).Where(user => user.Name.Contains(SearchText)));
            }
            catch (SessionExpiredException)
            {
                await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
                await Shell.Current.GoToAsync("SignInView");
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }
    }

    [RelayCommand]
    private async Task TapBanned()
    {
        await Shell.Current.GoToAsync(nameof(BannedListView));
    }

}
