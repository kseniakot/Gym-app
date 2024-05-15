namespace Gym.View;

public partial class TestPayment : ContentPage
{
    public TestPayment()
    {
        var webView = new WebView
        {
            Source = "https://yoomoney.ru/checkout/payments/v2/contract?orderId=2dd5977c-000f-5000-9000-1fe1555a2673"
        };
        webView.Navigating += (s, e) =>
        {
            if (e.Url.StartsWith("https://stirred-lightly-cattle.ngrok-free.app/users/resetpassword/"))
            {
                // Handle the redirect...
                e.Cancel = true;

                // Navigate back to the previous page
                Shell.Current.Navigation.PopAsync();
            }
        };

        Content = webView;
    }
}