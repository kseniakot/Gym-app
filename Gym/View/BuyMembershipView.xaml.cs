using Gym.Model;
using Gym.ViewModel;
namespace Gym.View;


[QueryProperty(nameof(MembershipId), "MembershipId")]

public partial class BuyMembershipView : ContentPage
{


    BuyMembershipViewModel _buyMembershipViewModel;
    string _membershipId;

    public string MembershipId
    {
        set
        {
            _membershipId = value;
            _buyMembershipViewModel.MembershipId = int.Parse(_membershipId);
            OnPropertyChanged();
        }

        get => _membershipId;

    }

    public BuyMembershipView(BuyMembershipViewModel buyMembershipViewModel)
	{
        _buyMembershipViewModel = buyMembershipViewModel;
		InitializeComponent();
        BindingContext = _buyMembershipViewModel;

        ///
        var viewModel = (BuyMembershipViewModel)BindingContext;

        // Listen to the LoadPaymentPageRequested event
        viewModel.LoadPaymentPageRequested += LoadPaymentPage;
    }

    private void LoadPaymentPage(string url)
    {
        // Create a WebView
        var webView = new WebView()
        {
            HeightRequest = Application.Current.MainPage.Height * 0.8,
        };

        // Handle the Navigating event
        webView.Navigating += (s, e) =>
        {
            if (e.Url.StartsWith("https://www.paymentgateway.com/paymentfinished"))
            {
                // Cancel the navigation
                e.Cancel = true;

                // Navigate back to the previous page
                Shell.Current.Navigation.PopAsync();
            }
           
        };
        webView.Source = url;
        MainLayout.Children.Clear();
        MainLayout.Children.Add(webView);
    }
 }