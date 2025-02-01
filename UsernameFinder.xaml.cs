using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class UsernameFinder : ContentPage
    {
        private Dictionary<string, WebsiteInfo> websites;
        private readonly ObservableCollection<AccountResult> results;

        public UsernameFinder()
        {
            InitializeComponent();
            websites = new Dictionary<string, WebsiteInfo>();
            InitializeAsync().ConfigureAwait(false);
            results = new ObservableCollection<AccountResult>();
            ResultsCollectionView.ItemsSource = results;
        }

        private async Task InitializeAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("http://192.168.0.160:5062/api/SocialMediaServices");
                    if (response.IsSuccessStatusCode)
                    {
                        var websiteList = await response.Content.ReadFromJsonAsync<List<WebsiteResponse>>();
                        if (websiteList != null)
                        {
                            websites = websiteList.ToDictionary(
                                w => w.Name,
                                w => new WebsiteInfo 
                                { 
                                    ErrorType = w.ErrorType,
                                    ErrorMessage = w.ErrorMessage,
                                    Url = w.UrlTemplate
                                }
                            );
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to parse website data from API", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to retrieve website data from API", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error retrieving website data: {ex.Message}", "OK");
                
                // init empty dictionary if API call fails
                websites = new Dictionary<string, WebsiteInfo>();
            }
        }

        private async void OnCheckWebsitesClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                await DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }

            results.Clear();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(8);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                var tasks = new List<Task>();
                foreach (var website in websites)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var result = await CheckWebsiteAsync(username, website.Key, website.Value, httpClient);
                        if (result != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() => results.Add(result));
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }

       

        private async Task<AccountResult> CheckWebsiteAsync(string username, string websiteName, WebsiteInfo websiteInfo, HttpClient httpClient)
        {
            try
            {
                var url = websiteInfo.Url.Replace("{}", username);
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return new AccountResult { WebsiteName = websiteName, ProfileUrl = url };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking {websiteName}: {ex.Message}");
            }
            return null;
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    public class AccountResult
    {
        public string WebsiteName { get; set; }
        public string ProfileUrl { get; set; }
    }

    public class WebsiteInfo
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string Url { get; set; }
    }

    public class WebsiteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string UrlTemplate { get; set; }
    }
}
