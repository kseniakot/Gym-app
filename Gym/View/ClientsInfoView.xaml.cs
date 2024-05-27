using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(WorkHourId), "WorkHourId")]
public partial class ClientsInfoView : ContentPage
{
    ClientsInfoViewModel _ClientsInfoViewModel;
    string _WorkHourId;

    public string WorkHourId
    {
        set
        {
            _WorkHourId = value;
            Debug.WriteLine($"WorkHourId: {_WorkHourId}");
            _ClientsInfoViewModel.WorkHourId = int.Parse(_WorkHourId);
            OnPropertyChanged();
        }

        get => _WorkHourId;

    }

    public ClientsInfoView(ClientsInfoViewModel ClientsInfoViewModel)
    {
        _ClientsInfoViewModel = ClientsInfoViewModel;
        InitializeComponent();
        BindingContext = _ClientsInfoViewModel;

      
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        _ClientsInfoViewModel.LoadWorkHourCommand.Execute(null);
    }


}