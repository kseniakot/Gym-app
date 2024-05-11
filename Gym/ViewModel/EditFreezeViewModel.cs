using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
using Gym.Exceptions;
using System.Collections.ObjectModel;
namespace Gym.ViewModel;

public partial class EditFreezeViewModel : ObservableObject
{
    [ObservableProperty]
    private Freeze? _Freeze;
    [ObservableProperty]
    private ObservableCollection<Freeze> _freezes;
    // [ObservableProperty]
    private Freeze _selectedFreeze;

    public Freeze SelectedFreeze
    {
        get { return _selectedFreeze; }
        set
        {
            _selectedFreeze = value;
            //Freeze.Freeze = value;
           

            //Debug.WriteLine(Freeze.Freeze.Id);
        }
    }

    private int _FreezeId;
    readonly WebService webService;
    public EditFreezeViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();

    }

    private async Task InitializeAsync()
    {
        try
        {
            Freezes = new ObservableCollection<Freeze>(await webService.GetAllFreezes());

            //Freezes.Insert(0, new Freeze { Id = 0, Name = "None" });
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

    public int FreezeId
    {
        get { return _FreezeId; }
        set
        {
            _FreezeId = value;
            OnPropertyChanged(nameof(FreezeId));

            // Load the Freeze when the FreezeId is set
            LoadFreeze();
        }
    }

    private async Task LoadFreeze()
    {
        try
        {
            Freeze = await webService.GetFreezeById(FreezeId);
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

    private static bool IsAnyNullOrEmpty(object Freeze)
    {
        var propertiesToCheck = new[] { "Name", "Months", "Price" };

        return Freeze.GetType()
            .GetProperties()
            .Where(pt => propertiesToCheck.Contains(pt.Name) && (pt.PropertyType == typeof(string) || pt.PropertyType == typeof(int?) || pt.PropertyType == typeof(decimal?)))
            .Select(v => v.GetValue(Freeze))
            .Any(value => value == null || string.IsNullOrWhiteSpace(value.ToString()));
    }

    private async Task<bool> DoesFreezeExist(Freeze Freeze)
    {
        try
        {
            return await webService.DoesFreezeExistAsync(Freeze);
        }
        catch (SessionExpiredException)
        {
            await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please sign in again.", "Ok");
            await Shell.Current.GoToAsync("SignInView");
            Application.Current.MainPage = new AppShell();
            return false;

        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Something went wrong", "Please try again", "Ok");
            return false;
        }
    }


    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }



    [RelayCommand]
    private async Task EditAsync()
    {
        if (IsAnyNullOrEmpty(Freeze))
        {
            Freeze = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (await DoesFreezeExist(Freeze))
        {
            // Freeze = new();
            await Shell.Current.DisplayAlert("Such Freeze already exists", "Make some changes or go back", "Ok");
        }
        else
        {
            try
            {
                await webService.EditFreeze(Freeze);
                await Shell.Current.DisplayAlert("Memebrship edited successfully", "Press Ok and return to the Freeze list", "Ok");

                BackCommand.Execute(null);
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
}