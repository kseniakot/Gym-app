using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
namespace Gym.ViewModel;

public partial class ClientsInfoViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<User> _clients;
   
    private int _WorkHourId;
    readonly WebService webService;
    public ClientsInfoViewModel(WebService webService)
    {
        this.webService = webService;

    }

 

    public int WorkHourId
    {
        get { return _WorkHourId; }
        set
        {
            _WorkHourId = value;
            OnPropertyChanged(nameof(WorkHourId));

            // Load the WorkHour when the WorkHourId is set
            //LoadWorkHour();
        }
    }

    [RelayCommand]
    private async Task LoadWorkHour()
    {
        try
        {
            Clients = new ObservableCollection<User>(await webService.GetWorkHourClients(WorkHourId));
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
