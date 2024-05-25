using Gym.Model;
using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;

[QueryProperty(nameof(DateString), "DateString")]

public partial class SelectTimeView : ContentPage
{
    SelectTimeViewModel _selectTimeViewModel;
    string _dateString;

    public string DateString
    {
        set
        {
            Debug.WriteLine(value);
            _dateString = value;
            _selectTimeViewModel.Date = DateTime.Parse(_dateString);
            OnPropertyChanged();
        }

        get => _dateString;

    }



    public SelectTimeView(SelectTimeViewModel selectTimeViewModel)
	{
        _selectTimeViewModel = selectTimeViewModel;
		InitializeComponent();
		BindingContext = selectTimeViewModel;
	}
}