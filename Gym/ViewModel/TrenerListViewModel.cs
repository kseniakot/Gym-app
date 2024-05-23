using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using Gym.Model;
using Gym.Services;
using Gym.Exceptions;
using System.Runtime.CompilerServices;
using System.Diagnostics;


namespace Gym.ViewModel;

public partial class TrenerListViewModel : ObservableObject
{

    [ObservableProperty]
    private ObservableCollection<Trener> _treners;

    [ObservableProperty]
    private Trener _selectedTrener;


    readonly WebService webService;
    public TrenerListViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Treners = new ObservableCollection<Trener>((await webService.GetAllTreners()));
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
    private async Task SelectTrenerAsync()
    {
        if (SelectedTrener == null) return;
        await Shell.Current.GoToAsync($"TrenerView?TrenerId={SelectedTrener.Id}");
        Debug.WriteLine(SelectedTrener.Id);
    }
}