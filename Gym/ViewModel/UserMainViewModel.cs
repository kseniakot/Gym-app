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

        //[ObservableProperty]
        //private WorkHour selectedWorkout;

        private WorkHour _selectedWorkout;
        public WorkHour SelectedWorkout
        {
            get { return _selectedWorkout; }
            set
            {
                if (_selectedWorkout != value)
                {
                    _selectedWorkout = value;
                    OnPropertyChanged();

                   
                    SelectedWorkout = null;
                }
            }
        }


        [ObservableProperty]
        private ObservableCollection<WorkHour> workouts;
        public UserMainViewModel(WebService webService)
        {
            this.webService = webService;
        }

        private async Task InitializeAsync()
        {
            User = await webService.GetUserFromToken();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            try
            {
                await InitializeAsync();
                Workouts = new ObservableCollection<WorkHour>(await webService.GetUserWorkouts(User.Id, DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                Workouts = new ObservableCollection<WorkHour>();
            }
        }
    }
}
