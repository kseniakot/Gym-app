using Gym.ViewModel;
namespace Gym.View;

public partial class TrenerScheduleView : ContentPage
{
    TrenerScheduleViewModel _trenerScheduleViewModel;

    public TrenerScheduleView(TrenerScheduleViewModel trenerScheduleViewModel)
	{
		InitializeComponent();
        _trenerScheduleViewModel = trenerScheduleViewModel;
		BindingContext = trenerScheduleViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Assuming you have a ViewModel property in your Page class
        _trenerScheduleViewModel.LoadDataCommand.Execute(null);
    }
}