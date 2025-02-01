using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MauiApp1
{
    public partial class GithubSearch : ContentPage
    {
        private readonly GitHubService _gitHubService;
        private readonly ObservableCollection<GitHubResult> _gitHubResults;

        public GithubSearch()
        {
            InitializeComponent();
            _gitHubService = new GitHubService();
            _gitHubResults = new ObservableCollection<GitHubResult>();
            GitHubResultsCollectionView.ItemsSource = _gitHubResults;
        }

        private async void OnGitHubSearchClicked(object sender, EventArgs e)
        {
            var input = GitHubSearchEntry.Text;

            if (string.IsNullOrEmpty(input))
            {
                await DisplayAlert("Error", "Please enter a GitHub username or email.", "OK");
                return;
            }

            _gitHubResults.Clear();

            if (input.Contains("@"))
            {
                // Search by email
                var username = await _gitHubService.FindUserByEmail(input);
                if (!string.IsNullOrEmpty(username))
                {
                    await DisplayAlert("Success", $"Username found: {username}", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No username found for the given email.", "OK");
                }
            }
            else
            {
                // Search emails by username
                var results = await _gitHubService.FindEmailsByUsername(input, ignoreForks: true);
                foreach (var result in results)
                {
                    _gitHubResults.Add(result);
                }

                if (_gitHubResults.Count == 0)
                {
                    await DisplayAlert("Error", "No emails found for the given username.", "OK");
                }
            }
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    public class GitHubService
    {
        private const string ApiUrl = "https://api.github.com";
        private readonly HttpClient _client;

        public GitHubService()
        {
            _client = new HttpClient
            {
                DefaultRequestHeaders =
                 {
                { "Accept", "application/vnd.github.v3+json" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.141 Safari/537.36" }
            }
            };
        }

        public async Task<ObservableCollection<GitHubResult>> FindEmailsByUsername(string username, bool ignoreForks)
        {
            var emails = new ObservableCollection<GitHubResult>();
            var url = $"{ApiUrl}/users/{username}/repos?per_page=100";

            var repos = await ApiCall(url);
            if (repos is JsonArray repoArray)
            {
                foreach (var repo in repoArray)
                {
                    var isFork = repo["fork"]?.GetValue<bool>() ?? false;
                    if (ignoreForks && isFork) continue;

                    var repoName = repo["name"]?.ToString();
                    var commitsUrl = $"{ApiUrl}/repos/{username}/{repoName}/commits?per_page=100";
                    var commits = await ApiCall(commitsUrl);

                    if (commits is JsonArray commitArray)
                    {
                        foreach (var commit in commitArray)
                        {
                            var author = commit["commit"]?["author"];
                            if (author != null)
                            {
                                var email = author["email"]?.ToString();
                                var name = author["name"]?.ToString();

                                if (!string.IsNullOrEmpty(email) && !emails.Any(e => e.Email == email))
                                {
                                    emails.Add(new GitHubResult { Email = email, Name = name });
                                }
                            }
                        }
                    }
                }
            }

            return emails;
        }

        public async Task<string> FindUserByEmail(string email)
        {
            var url = $"{ApiUrl}/search/users?q={email}";
            var result = await ApiCall(url);

            if (result is JsonObject obj && obj["total_count"]?.GetValue<int>() > 0)
            {
                return obj["items"]?[0]?["login"]?.ToString();
            }

            return null;
        }

        private async Task<JsonNode> ApiCall(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content);
            }
            catch
            {
                return null;
            }
        }
    }

    public class GitHubResult
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

   
}
