using Gym.Model;
using Gym.ViewModel;
namespace Gym.View;

[QueryProperty(nameof(MembershipId), "MembershipId")]
public partial class ActivateMembershipView : ContentPage
{
    ActivateMembershipViewModel _activateMembershipViewModel;
    string _membershipId;
    public ActivateMembershipView(ActivateMembershipViewModel activateMembershipViewModel)
	{
        _activateMembershipViewModel = activateMembershipViewModel;
		InitializeComponent();
        BindingContext = _activateMembershipViewModel;
	}

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _activateMembershipViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }
}