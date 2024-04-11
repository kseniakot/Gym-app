using Gym.Model;
using Gym.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Gym.View;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Gym.ViewModel;

public partial class MembershipListViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership _selectedMembership;
    [ObservableProperty]
    private ObservableCollection<Membership> _memberships;

    [ObservableProperty]
    private string _searchText;

    readonly DataBaseService _dbService;
    public MembershipListViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
        Memberships = new ObservableCollection<Membership>(dbService.GetAllMemberships());
    }
    [RelayCommand]
    private async Task RemoveMembershipAsync()
    {
        if (SelectedMembership != null)
        {
            //  await DatabaseService<User>.RemoveColumnAsync(SelectedUser.Id);
            _dbService.DeleteMembership(SelectedMembership);
            Memberships.Remove(SelectedMembership);

            SelectedMembership = null;
        }
        else
        {
            await Shell.Current.DisplayAlert("No membership selected", "Please select and try again.", "Ok");
        }
    }


    [RelayCommand]
    private async Task AddMembershipAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddMembershipView));
    }

    
    [RelayCommand]
    private void LoadData()
    {
        Memberships = new ObservableCollection<Membership>(_dbService.GetAllMemberships());
    }

    [RelayCommand]
    private void SearchMembership()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Memberships = new ObservableCollection<Membership>(_dbService.GetAllMemberships());
        }
        else
        {
            Memberships = new ObservableCollection<Membership>(_dbService.GetAllMemberships().Where(membership => membership.Name.Contains(SearchText)));
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