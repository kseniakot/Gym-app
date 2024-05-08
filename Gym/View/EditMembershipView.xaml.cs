using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(MembershipId), "MembershipId")]
public partial class EditMembershipView : ContentPage
{
    EditMembershipViewModel _editMembershipViewModel;
    string _membershipId;   

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _editMembershipViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;
        
    }

    



    public EditMembershipView(EditMembershipViewModel editMembershipViewModel)
    {
        _editMembershipViewModel = editMembershipViewModel;
        InitializeComponent();
        BindingContext = _editMembershipViewModel;
        
        //Debug.WriteLine("EditMembershipView");
        //Debug.WriteLine(MembershipId);
    }


}