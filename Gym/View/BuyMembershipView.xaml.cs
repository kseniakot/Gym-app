using Gym.Model;
using Gym.ViewModel;
namespace Gym.View;


[QueryProperty(nameof(MembershipId), "MembershipId")]

public partial class BuyMembershipView : ContentPage
{


    BuyMembershipViewModel _buyMembershipViewModel;
    string _membershipId;

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _buyMembershipViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }

    public BuyMembershipView(BuyMembershipViewModel buyMembershipViewModel)
	{
        _buyMembershipViewModel = buyMembershipViewModel;
		InitializeComponent();
        BindingContext = _buyMembershipViewModel;
	}
}