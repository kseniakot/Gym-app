using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Gym.ViewModel;

public partial class TrenerScheduleViewModel : ObservableObject
{


    [ObservableProperty]
    private DateTime _selectedDate = DateTime.UtcNow;
   // [ObservableProperty]
    private WorkHour _selectedHour;

    public WorkHour SelectedHour
    {
        get { return _selectedHour; }
        set
        {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));

                IsButtonEnabled = true;
            
        }
    }

  
    [ObservableProperty]
    private ObservableCollection<WorkHour> _workHours;

   
    [ObservableProperty]
    private bool isButtonEnabled = false;


 
    readonly WebService webService;
    public TrenerScheduleViewModel(WebService webService)
    {
        this.webService = webService;
        //LoadTrenerSchedule();

    }


    //private async Task LoadTrenerSchedule()
    //{
    //    try
    //    {
    //        WorkHours = new ObservableCollection<WorkHour>(await webService.GetTrenerWorkHours((await webService.GetUserFromToken()).Id, DateTime.Today)); 
    //    }
    //    catch (SessionExpiredException)
    //    {
    //        await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
    //        await Shell.Current.GoToAsync("SignInView");
    //        Application.Current.MainPage = new AppShell();
    //    }
    //    catch (Exception e)
    //    {
    //        WorkHours = new ObservableCollection<WorkHour>();
    //    }

    //}

    [RelayCommand]
    private async Task LoadData()
    {
        await DateSelectedAsync();
    }

    [RelayCommand]
    public async Task DateSelectedAsync()
    {
        try
        {
            WorkHours = new ObservableCollection<WorkHour>(await webService.GetTrenerWorkHours((await webService.GetUserFromToken()).Id, SelectedDate));

        }
        catch (Exception e)
        {
            WorkHours = new ObservableCollection<WorkHour>();
        }

    }



    [RelayCommand]
    private async void AddTime()
    {
        string dateString = SelectedDate.ToString("d");
        await Shell.Current.GoToAsync($"SelectTime?DateString={dateString}");
    }

    [RelayCommand]
    private async Task RemoveHour()
    {
        IsButtonEnabled = false;
        try
        {
            await webService.RemoveWorkHour((await webService.GetUserFromToken()).Id, SelectedHour.Start);
            WorkHours = new ObservableCollection<WorkHour>(await webService.GetTrenerWorkHours((await webService.GetUserFromToken()).Id, SelectedDate));
        }
        catch(Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
        }
    }




}