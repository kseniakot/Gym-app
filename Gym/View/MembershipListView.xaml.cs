using Gym.ViewModel;
namespace Gym.View;

public partial class MembershipListView : ContentPage
{
    readonly MembershipListViewModel _membershipListViewModel;

    public MembershipListView(MembershipListViewModel membershipListViewModel)
    {
        _membershipListViewModel = membershipListViewModel;
        InitializeComponent();
        BindingContext = _membershipListViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _membershipListViewModel.LoadDataCommand.Execute(null);
    }
}