using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Gym.Exceptions;

namespace Gym.ViewModel;

public partial class MembershipListViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership _selectedMembership;
    [ObservableProperty]
    private ObservableCollection<Membership> _memberships;

    [ObservableProperty]
    private string _searchText;

    readonly WebService webService;
    public MembershipListViewModel(WebService webService)
    {
        this.webService = webService;
        InitializeAsync();
       
    }

    private async Task InitializeAsync()
    {
        try
        {
            Memberships = new ObservableCollection<Membership>(await webService.GetAllMemberships());
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

    [RelayCommand]
    private async Task RemoveMembershipAsync()
    {
        if (SelectedMembership != null)
        {
            try
            {
                await webService.DeleteMembership(SelectedMembership);
                Memberships.Remove(SelectedMembership);
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
            finally
            {
                SelectedMembership = null;
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("No user selected", "Please select and try again.", "Ok");
        }
    }


    [RelayCommand]
    private async Task AddMembershipAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddMembershipView));
    }


    [RelayCommand]
    private async Task LoadData()
    {
        await InitializeAsync();
    }
    [RelayCommand]
    private async Task SearchMembershipAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            try
            {
                Memberships = new ObservableCollection<Membership>(await webService.GetAllMemberships());
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
        else
        {
            try
            {
                Memberships = new ObservableCollection<Membership>((await webService.GetAllMemberships()).Where(membership => membership.Name.Contains(SearchText)));
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

    [RelayCommand]
    private async Task EditMembershipAsync()
    {
        if (SelectedMembership == null)
        {
            await Shell.Current.DisplayAlert("No membership selected", "Please select and try again.", "Ok");
           
        }
        else {
            //var navigationParameter = new ShellNavigationQueryParameters
            //{
            //    { "MembershipId", SelectedMembership.Id }
            //};
            await Shell.Current.GoToAsync($"EditMembershipView?MembershipId={SelectedMembership.Id}");
            SelectedMembership = null;
         }
    }

}