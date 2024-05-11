using Gym.Model;
using Gym.ViewModel;
namespace Gym.View;

[QueryProperty(nameof(MembershipId), "MembershipId")]
public partial class CancelFreezeView : ContentPage
{
    CancelFreezeViewModel _cancelFreezeViewModel;
    string _membershipId;
    public CancelFreezeView(CancelFreezeViewModel cancelFreezeViewModel)
    {
        _cancelFreezeViewModel = cancelFreezeViewModel;
        InitializeComponent();
        BindingContext = _cancelFreezeViewModel;
    }

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _cancelFreezeViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }
}