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

public partial class TrenerListAdminViewModel : ObservableObject
{
    [ObservableProperty]
    private Trener _selectedTrener;
    [ObservableProperty]
    private ObservableCollection<Trener> _Treners;
    

    [ObservableProperty]
    private string _searchText;

    readonly WebService webService;
    public TrenerListAdminViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Treners = new ObservableCollection<Trener>(await webService.GetAllTreners());
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
    private async Task RemoveTrenerAsync()
    {
        if (SelectedTrener != null)
        {
            try
            {
                await webService.DeleteTrener(SelectedTrener);
                Treners.Remove(SelectedTrener);
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
                SelectedTrener = null;
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("No user selected", "Please select and try again.", "Ok");
        }
    }


    [RelayCommand]
    private async Task AddTrenerAsync()
    {
       await Shell.Current.GoToAsync(nameof(AddTrenerView));
    }


    [RelayCommand]
    private async Task LoadData()
    {
        await InitializeAsync();
    }
    [RelayCommand]
    private async Task SearchTrenerAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            try
            {
                Treners = new ObservableCollection<Trener>(await webService.GetAllTreners());
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
                Treners = new ObservableCollection<Trener>((await webService.GetAllTreners()).Where(Trener => Trener.Name.ToLower().Contains(SearchText.ToLower())));
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

    

}