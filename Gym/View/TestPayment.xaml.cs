using Gym.Model;
using System.Diagnostics;

namespace Gym.View;

[QueryProperty(nameof(Url), "Url")]
public partial class TestPayment : ContentPage
{
    string _url;
    public string Url
    {
        set
        {
            _url = value;
            OnPropertyChanged();
        }

        get => _url;

    }
   
    public TestPayment()
    {
        var webView = new WebView
        {
            Source = Url,
        };
        webView.Navigating += (s, e) =>
        {
            if (e.Url.StartsWith("https://www.google.com"))
            {
                // Handle the redirect...
                e.Cancel = true;

                // Navigate back to the previous page
                Shell.Current.Navigation.PopAsync();
            }
        };
        Debug.WriteLine("TestPayment");
        Debug.WriteLine(Url);
        Content = webView;
    }
}