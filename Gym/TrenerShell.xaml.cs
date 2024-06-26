using Gym.ViewModel;
using System.Diagnostics;
namespace Gym.View;


public partial class TrenerShell : Shell
{

    public TrenerShell()
    {
        Routing.RegisterRoute("SignInView", typeof(SignInView));
        Routing.RegisterRoute("SelectTime", typeof(SelectTimeView));
        Routing.RegisterRoute("ClientsInfoView", typeof(ClientsInfoView));
        InitializeComponent();



    }



    private async void LogOut_Button(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SignInView");
        Application.Current.MainPage = new AppShell();
    }

}