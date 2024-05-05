using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(MembershipId), "MembershipId")]
public partial class MembershipView : ContentPage
{

    MembershipViewModel _MembershipViewModel;
    string _membershipId;

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _MembershipViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }





    public MembershipView(MembershipViewModel MembershipViewModel)
    {
        _MembershipViewModel = MembershipViewModel;
        InitializeComponent();
        BindingContext = _MembershipViewModel;

       
    }
}