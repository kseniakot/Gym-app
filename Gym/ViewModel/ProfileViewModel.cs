using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using Gym.Exceptions;
using Microsoft.Maui.Controls;
//using Xamarin.KotlinX.Coroutines.Channels;
using Microsoft.Maui.Controls;

namespace Gym.ViewModel;

public partial class ProfileViewModel : ObservableObject
{
	[ObservableProperty]
	private ImageSource? _myImageSource;

    readonly WebService webService;
    public ProfileViewModel(WebService webService)
    {
        this.webService = webService;
      
    }

    [RelayCommand]
    private async Task SelectImage()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick Image Please",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
            return;

        var stream = await result.OpenReadAsync();

       MyImageSource = ImageSource.FromStream(() => stream);

    }
}