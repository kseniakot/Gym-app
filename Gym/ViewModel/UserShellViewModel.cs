using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using CoreBluetooth;
using Gym.Services;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class UserShellViewModel : ObservableObject
{

	[ObservableProperty]
	private bool _isMember;

    private readonly WebService _webService;
    public UserShellViewModel(WebService webService)
	{
     
        _webService = webService;
       SetIsMember();
        
    }

    private async void SetIsMember()
    {
        if (await _webService.CheckUserOrMember((await _webService.GetUserFromToken()).Email))
            {
            IsMember = true;
        }
        else
        {
            IsMember = false;
        }
    }
}