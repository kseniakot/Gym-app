using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MimeKit.Cryptography;
namespace Gym.ViewModel;



public partial class TrenerScheduleViewModel : ObservableObject
{


    [ObservableProperty]
    private DateTime _selectedDate = DateTime.UtcNow;

    [ObservableProperty]
    private DateTime _selectedDateCopy;

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

    [ObservableProperty]
    private bool isCopyEnabled = false;



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
            IsButtonEnabled = false;
           
                var workHours = await webService.GetTrenerWorkHours((await webService.GetUserFromToken()).Id, SelectedDate);
                WorkHours = new ObservableCollection<WorkHour>(workHours);
           
            IsCopyEnabled = true;


        }
        catch
        {
            WorkHours = [];
            IsCopyEnabled= false;   
        }

    }

    [ObservableProperty]
    private bool isSelectVisible = false;

    [RelayCommand]
    private async void AddTime()
    {
        string dateString = SelectedDate.ToString("d");
        await Shell.Current.GoToAsync($"SelectTime?DateString={dateString}");
    }

    [RelayCommand]
    private async void CopyTo()
    {
        IsSelectVisible = true;
    }

    [RelayCommand]
    private async Task CopyToSelected()
    {
        IsCopyEnabled = false;
        if (SelectedDate<SelectedDateCopy)
        {
            try
            {
                await webService.CopyWorkDay((await webService.GetUserFromToken()).Id, SelectedDate, SelectedDateCopy);
                await Shell.Current.DisplayAlert("Success", "Data has been copied successfully", "Ok");
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                
            }

            IsSelectVisible = false;
        }
        
    }

    [RelayCommand]
    private async Task RemoveHour()
    {
        IsButtonEnabled = false;
        if (SelectedHour != null)
        {
            try
            {
                await webService.RemoveWorkHour((await webService.GetUserFromToken()).Id, SelectedHour.Start);
                WorkHours = new ObservableCollection<WorkHour>(await webService.GetTrenerWorkHours((await webService.GetUserFromToken()).Id, SelectedDate));
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }
    }

    [RelayCommand]
    public async Task ClientsInfo()
    {
        if(SelectedHour.WorkHourClients.Count==0)
        {
            return;
        }
        await Shell.Current.GoToAsync($"ClientsInfoView?WorkHourId={SelectedHour.Id}");
        Debug.WriteLine(SelectedHour.Id);
        SelectedHour = null;
    }




}