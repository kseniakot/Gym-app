using Gym.Model;
using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(MembershipId), "MembershipId")]
public partial class FreezeMembershipView : ContentPage
{

    FreezeMembershipViewModel _freezeMembershipViewModel;
    string _membershipId;
    public FreezeMembershipView(FreezeMembershipViewModel freezeMembershipViewModel)
    {
        _freezeMembershipViewModel = freezeMembershipViewModel;
        InitializeComponent();
        BindingContext = _freezeMembershipViewModel;
    }

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _freezeMembershipViewModel.MembershipId = int.Parse(_membershipId);
            Debug.WriteLine("MembershipId: " + _membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }
}