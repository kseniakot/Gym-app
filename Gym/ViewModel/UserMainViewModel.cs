using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym.Services;
using Gym.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Gym.ViewModel
{
    public partial class UserMainViewModel : ObservableObject
    {
        readonly WebService webService;
        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private WorkHour selectedWorkout;

        [ObservableProperty]
        private ObservableCollection<WorkHour> workouts;
        public UserMainViewModel(WebService webService)
        {
            this.webService = webService;
           InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            User = await webService.GetUserFromToken();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            //try
            //{
            //    Workouts = new ObservableCollection<WorkHour>(await webService.GetUserWorkouts((await webService.GetUserFromToken()).Id, DateTime.UtcNow));
            //}
            //catch (Exception ex)
            //{
            //    Workouts = new ObservableCollection<WorkHour>();
            //}
        }
    }
}
