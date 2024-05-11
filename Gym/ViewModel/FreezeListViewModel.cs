using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Gym.Exceptions;

namespace Gym.ViewModel;

public partial class FreezeListViewModel : ObservableObject
{
    [ObservableProperty]
    private Freeze _selectedFreeze;
    [ObservableProperty]
    private ObservableCollection<Freeze> _Freezes;

    [ObservableProperty]
    private string _searchText;

    readonly WebService webService;
    public FreezeListViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Freezes = new ObservableCollection<Freeze>(await webService.GetAllFreezes());
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
    private async Task RemoveFreezeAsync()
    {
        if (SelectedFreeze != null)
        {
            try
            {
                await webService.DeleteFreeze(SelectedFreeze);
                Freezes.Remove(SelectedFreeze);
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
                SelectedFreeze = null;
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("No user selected", "Please select and try again.", "Ok");
        }
    }


    [RelayCommand]
    private async Task AddFreezeAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddFreezeView));
    }


    [RelayCommand]
    private async Task LoadData()
    {
        await InitializeAsync();
    }
    [RelayCommand]
    private async Task SearchFreezeAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            try
            {
                Freezes = new ObservableCollection<Freeze>(await webService.GetAllFreezes());
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
                Freezes = new ObservableCollection<Freeze>((await webService.GetAllFreezes()).Where(Freeze => Freeze.Name.ToLower().Contains(SearchText.ToLower())));
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
    private async Task EditFreezeAsync()
    {
        if (SelectedFreeze == null)
        {
            await Shell.Current.DisplayAlert("No Freeze selected", "Please select and try again.", "Ok");

        }
        else
        {
            var navigationParameter = new ShellNavigationQueryParameters
            {
                { "FreezeId", SelectedFreeze.Id }
            };
            await Shell.Current.GoToAsync($"EditFreezeView?FreezeId={SelectedFreeze.Id}");
            SelectedFreeze = null;
        }
    }

}