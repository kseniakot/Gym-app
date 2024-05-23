using Gym.ViewModel;
namespace Gym.View;

public partial class TrenerListAdminView : ContentPage
{
    readonly TrenerListAdminViewModel _trenerListAdminViewModel;

    public TrenerListAdminView(TrenerListAdminViewModel trenerListAdminViewModel)
    {
        _trenerListAdminViewModel = trenerListAdminViewModel;
        InitializeComponent();
        BindingContext = trenerListAdminViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _trenerListAdminViewModel.LoadDataCommand.Execute(null);
    }
}