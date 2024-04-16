using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using Gym.Exceptions;   
namespace Gym.ViewModel;

public partial class BannedListViewModel : ObservableObject
{
    [ObservableProperty]
    private User _selectedUser;
    [ObservableProperty]
    private ObservableCollection<User> _users;

    [ObservableProperty]
    private string _searchText;

    readonly WebService webService;
    public BannedListViewModel(WebService webService)
    {
        this.webService = webService;
       InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            Users = new ObservableCollection<User>(await webService.GetBannedUsers());
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



    [RelayCommand]
    private async Task UnbanUser()
    {
       await webService.BanUser(SelectedUser.Id);
        Users.Remove(SelectedUser);
        await Shell.Current.DisplayAlert("SUCCESS", "The user was unbanned successfully", "OK");

    }
    [RelayCommand]
    private async Task LoadData()
    {
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task SearchUser()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            try
            {
                Users = new ObservableCollection<User>(await webService.GetBannedUsers());
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
        else
        {
            try
            {
                Users = new ObservableCollection<User>((await webService.GetBannedUsers()).Where(user => user.Name.Contains(SearchText)));
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