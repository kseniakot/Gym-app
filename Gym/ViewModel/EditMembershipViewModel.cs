using Gym.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Services;
using System.Diagnostics;
namespace Gym.ViewModel;

public partial class EditMembershipViewModel : ObservableObject
{
    [ObservableProperty]
    private Membership? _membership;

    private int _membershipId;
    readonly DataBaseService _dbService;
    public EditMembershipViewModel(DataBaseService dbService)
    {
        _dbService = dbService;
        
    }
    public int MembershipId
    {
        get { return _membershipId; }
        set
        {
            _membershipId = value;
            OnPropertyChanged(nameof(MembershipId));

            // Load the Membership when the MembershipId is set
            LoadMembership();
        }
    }

    private void LoadMembership()
    {
        Debug.WriteLine("LoadMembership");
        Debug.WriteLine(MembershipId);
        Membership = _dbService.GetMembershipById(MembershipId);
       // _dbService.DeleteMembership(Membership);
    }

    private static bool IsAnyNullOrEmpty(object membership)
    {
        return membership.GetType()
            .GetProperties()
            .Where(pt => pt.PropertyType == typeof(string))
            .Select(v => (string)v.GetValue(membership))
            .Any(value => string.IsNullOrWhiteSpace(value));
    }

    private bool IsMembershipExist(Membership membership) 
    {
        return _dbService.IsMembershipExist(membership);
    }

    [RelayCommand]
    private static async Task BackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

   

    [RelayCommand]
    private async Task EditAsync()
    {
        if (IsAnyNullOrEmpty(Membership))
        {
            Membership = new();
            await Shell.Current.DisplayAlert("There is an empty field", "Please fill it out and try again.", "Ok");
        }
        else if (IsMembershipExist(Membership))
        {
           // Membership = new();
            await Shell.Current.DisplayAlert("Such membership already exists", "Make some changes or go back", "Ok");
        }
        else
        {
            //await DatabaseService<User>.AddColumnAsync(User);
            _dbService.EditMembership(Membership);
            await Shell.Current.DisplayAlert("Memebrship edited successfully", "Press Ok and return to the membership list", "Ok");

            BackCommand.Execute(null);
        }


    }
}