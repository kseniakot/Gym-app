using CommunityToolkit.Mvvm.Input;
using Gym.ViewModel;
namespace Gym.View;

[QueryProperty(nameof(TrenerId), "TrenerId")]
public partial class TrenerView : ContentPage
{

    TrenerViewModel _trenerViewModel;
    string _trenerId;

    public string TrenerId
    {
        set
        {
            _trenerId = value;
            _trenerViewModel.TrenerId = int.Parse(_trenerId);
            OnPropertyChanged();
        }

        get => _trenerId;

    }

    public TrenerView(TrenerViewModel trenerViewModel)
    {
        _trenerViewModel = trenerViewModel;
        InitializeComponent();
        BindingContext = _trenerViewModel;


    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _trenerViewModel.LoadDataCommand.Execute(null);
    }


}