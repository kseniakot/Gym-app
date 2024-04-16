using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym.Services;
using Gym.Model;

namespace Gym.ViewModel
{
    public partial class UserMainViewModel : ObservableObject
    {
        readonly WebService webService;
        [ObservableProperty]
        private User _user;
        public UserMainViewModel(WebService webService)
        {
            this.webService = webService;
           InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            User = await webService.GetUserFromToken();
        }
    }
}
