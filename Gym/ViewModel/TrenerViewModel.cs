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
    private bool isButtonEnabled = false;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.UtcNow;

    [ObservableProperty]
    private DateTime _maxDate;

   
    private WorkHour _selectedHour;

    public WorkHour SelectedHour
    {
        get { return _selectedHour; }
        set
        {
            if (_selectedHour != value)
            {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));
                IsButtonEnabled = true;

            }
        }
    }



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
    private async Task LoadData()
    {
        MaxDate = DateTime.UtcNow.AddDays(6);
        await DateSelectedAsync();
    }

    [RelayCommand]
    public async Task DateSelectedAsync()
    {
        try
        {
            Workhours = new ObservableCollection<WorkHour>((await webService.GetAvailableWorkHoursByDateByTrenerId(TrenerId, SelectedDate)));
            IsButtonVisible = true;
        }
        catch
        {
            Workhours = new ObservableCollection<WorkHour>();
            IsButtonVisible = false;
        }

    }

    [RelayCommand]
    private async Task BookWorkout()

    {
        try
        {
            await webService.ApplyWorkout(Trener.Id, (await webService.GetUserFromToken()).Id, SelectedHour.Start);
            await Shell.Current.DisplayAlert("Success", "Time has been booked successfully", "OK");
           
            await DateSelectedAsync();
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
    private async Task ApplyWeekdays()
    {
        if (SelectedHour == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a worktime", "Ok");
            return;
        }
        try
        {
            string result = await webService.ApplyWorkoutByWeekday(TrenerId, (await webService.GetUserFromToken()).Id, SelectedHour.Start);
            SelectedHour = null;
            await Shell.Current.DisplayAlert("Result", $"{result.Trim('"')}", "OK");
           
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
        }
    }


}