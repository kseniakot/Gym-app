using Gym.Model;
using System.Diagnostics;

namespace Gym.View;

[QueryProperty(nameof(Url), "Url")]
public partial class TestPayment : ContentPage
{
    string _url;
    WebView webView;

    public string Url
    {
        set
        {
            _url = Uri.UnescapeDataString(value.Trim());
            Debug.WriteLine("Url: " + _url);    
            OnPropertyChanged();

            webView = new WebView
            {
                Source = _url,
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
            Content = webView;
        }

        get => _url;
    }

    public TestPayment()
    {
        Debug.WriteLine("TestPayment");
        Debug.Write("Plain text");
        Debug.WriteLine(Url);
    }
}