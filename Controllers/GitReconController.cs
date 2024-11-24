using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace OsintEyeWeb.Controllers
{
    public class GitReconController : Controller
    {
        private const string API_URL = "https://api.github.com";
        private static readonly Dictionary<string, string> HEADER = new Dictionary<string, string>
            {
                { "Accept", "application/vnd.github.v3+json" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.141 Safari/537.36" }
            };

        private readonly ILogger<GitReconController> _logger;
        private readonly HttpClient _client;

        public GitReconController(ILogger<GitReconController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FindEmailsByUsername(string username, string token, bool ignoreForks)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Username is required.");
                return View("Index");
            }

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            }

            // Log the request to ensure it's starting correctly
            _logger.LogInformation("Starting email search for username: {Username}", username);

            var emails = await GetEmailsByUsername(username, ignoreForks);

            _logger.LogInformation("Emails fetched for username {Username}: {EmailCount} emails found", username, emails.Count);

            // Pass results to ViewBag
            ViewBag.Results = emails.Select(e => new { Email = e.Email, Author = e.Author }).ToList();

            return View("Results");
        }

        private async Task<List<(string Email, string Author)>> GetEmailsByUsername(string username, bool ignoreForks)
        {
            var emails = new List<(string Email, string Author)>();
            var url = $"{API_URL}/users/{username}/repos?per_page=100";

            _logger.LogInformation("Calling API to fetch repos for username: {Username}, URL: {Url}", username, url);

            try
            {
                var result = await ApiCall(_client, url);

                if (result is JsonArray repos)
                {
                    _logger.LogInformation("Found {RepoCount} repositories for username: {Username}", repos.Count, username);

                    foreach (var repo in repos)
                    {
                        bool isFork = repo["fork"]?.GetValue<bool>() ?? false;
                        if (ignoreForks && isFork)
                        {
                            _logger.LogInformation("Skipping forked repo: {RepoName}", repo["name"]?.ToString());
                            continue;
                        }

                        var repoName = repo["name"]?.ToString();
                        var commitsUrl = $"{API_URL}/repos/{username}/{repoName}/commits?per_page=100";

                        _logger.LogInformation("Fetching commits for repo: {RepoName}, URL: {CommitsUrl}", repoName, commitsUrl);

                        var commits = await ApiCall(_client, commitsUrl);

                        if (commits is JsonArray commitArray)
                        {
                            _logger.LogInformation("Found {CommitCount} commits for repo: {RepoName}", commitArray.Count, repoName);

                            foreach (var commit in commitArray)
                            {
                                var author = commit["commit"]?["author"];
                                if (author != null)
                                {
                                    var email = author["email"]?.ToString();
                                    var authorName = author["name"]?.ToString();

                                    if (!string.IsNullOrEmpty(email) && !emails.Any(e => e.Email == email))
                                    {
                                        emails.Add((email, authorName));
                                        _logger.LogInformation("Found email: {Email} for author: {AuthorName}", email, authorName);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No repositories found for username: {Username}", username);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching emails for username {Username}.", username);
            }

            return emails;
        }

        // Finds a GitHub username by email
        [HttpPost]
        public async Task<IActionResult> FindUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View("Index");
            }

            // Log the email for which the username is being searched
            _logger.LogInformation("Starting username search for email: {Email}", email);

            string username = await FindUserNameByEmail(email);

            // Log the result of the search
            _logger.LogInformation("Username found for email {Email}: {Username}", email, username);

            // Pass the found username or a failure message to the view
            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.Result = new { Username = username };
                ViewBag.Message = "Query successful.";
            }
            else
            {
                ViewBag.Message = "No username found or failed to fetch data.";
            }

            return View("Results");
        }

        // Utility method to find a GitHub username by email
        private async Task<string> FindUserNameByEmail(string email)
        {
            var url = $"{API_URL}/search/users?q={email}";
            var result = await ApiCall(_client, url);

            if (result != null && result is JsonObject obj && obj["total_count"]?.GetValue<int>() > 0)
            {
                return obj["items"]?[0]?["login"]?.ToString();
            }

            return null;
        }

        // Utility method to make API calls
        private async Task<JsonNode> ApiCall(HttpClient client, string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var header in HEADER)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API call failed. URL: {Url}, StatusCode: {StatusCode}", url, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API call successful. URL: {Url}, Response: {Response}", url, content);
                return JsonNode.Parse(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during API call to {Url}", url);
                return null;
            }
        }
    }
}
