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
        private DateTime _selectedDate = DateTime.UtcNow;

        [ObservableProperty]
        bool isButtonEnabled = false;


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
                    IsButtonEnabled = true;
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
                Workouts = new ObservableCollection<WorkHour>(await webService.GetUserWorkouts(User.Id, SelectedDate));
            }
            catch (Exception ex)
            {
                Workouts = new ObservableCollection<WorkHour>();
            }
        }

        [RelayCommand]
        public async Task DateSelectedAsync()
        {
            try
            {


                Workouts = new ObservableCollection<WorkHour>(await webService.GetUserWorkouts(User.Id, SelectedDate));

            }
            catch
            {
                Workouts = [];
            }

        }

        [RelayCommand]
        public async Task CancelWorkoutAsync()
        {
            if(SelectedWorkout == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a workout to cancel", "Ok");
                return;
            }
            try
            {
                await webService.RemoveWorkHourClient(SelectedWorkout.Id, User.Id);
                Workouts.Remove(SelectedWorkout);
                SelectedWorkout = null;
                isButtonEnabled = false;
            }
            catch(Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }
    }
}
