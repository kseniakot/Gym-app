using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
namespace Gym.ViewModel;

public partial class TrenerViewModel : ObservableObject
{
    [ObservableProperty]
    private Trener? _Trener;

    [ObservableProperty]
    private bool isButtonVisible = false;

    [ObservableProperty]
    private DateTime _SelectedDate;
    [ObservableProperty] 
    private DateTime _SelectedHour;

    [ObservableProperty]
    private ObservableCollection<WorkHour> _workhours;


    private int _TrenerId;
    readonly WebService webService;
    public TrenerViewModel(WebService webService)
    {
        this.webService = webService;

    }

    public int TrenerId
    {
        get { return _TrenerId; }
        set
        {
            _TrenerId = value;
            Debug.WriteLine($"TrenerId set to {_TrenerId}");
            OnPropertyChanged(nameof(TrenerId));

            LoadTrener();
        }
    }

    private async Task LoadTrener()
    {
        try
        {
            Trener = await webService.GetTrenerById(TrenerId);
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
    public async Task DateSelectedAsync()
    {
        try
        {
            Workhours = new ObservableCollection<WorkHour>((await webService.GetAvailableWorkHoursByDateByTrenerId(TrenerId, SelectedDate)));
            IsButtonVisible = true;
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            Workhours = new ObservableCollection<WorkHour>();
            IsButtonVisible = false;
        }

    }



}