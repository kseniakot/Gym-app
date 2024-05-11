using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(FreezeId), "FreezeId")]
public partial class EditFreezeView : ContentPage
{
    EditFreezeViewModel _editFreezeViewModel;
    string _FreezeId;

    public string FreezeId
    {
        set
        {
            _FreezeId = value;
            _editFreezeViewModel.FreezeId = int.Parse(_FreezeId);
            OnPropertyChanged();
        }

        get => _FreezeId;

    }





    public EditFreezeView(EditFreezeViewModel editFreezeViewModel)
    {
        _editFreezeViewModel = editFreezeViewModel;
        InitializeComponent();
        BindingContext = _editFreezeViewModel;

    }


}