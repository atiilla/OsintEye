using System.Net.Http;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GitRecon.Models;
using Newtonsoft.Json;
using GitRecon.Controllers;
using System.Drawing;
using System.Windows.Media;

namespace GitRecon
{
    public partial class MainWindow : Window
    {




        private const string API_URL = "https://api.github.com";
        private static readonly Dictionary<string, string> HEADER = new Dictionary<string, string>

        {
            { "Accept", "application/vnd.github.v3+json" },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.141 Safari/537.36" }
        };
        private static readonly int DELAY = 3000; // Delay 3 seconds to avoid rate limiting

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LightThemeClick(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Resources/Styles.xaml", UriKind.Relative));
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Resources/Dark.xaml", UriKind.Relative));
        }

        // Event handler for querying by email
        private async void btnUsernameQuery_Click(object sender, RoutedEventArgs e)
        {
            string email = txEmail.Text;  // Get email from the TextBox

           
            // Display message if email is not provided
            if (string.IsNullOrEmpty(email))
            {
                DisplayMessage(UsernameStatusTextBlock, "Please enter an email.");
                return;
            }


            // Get username associated with the email
            var username = await FindUserNameByEmail(email);
            if (!string.IsNullOrEmpty(username))
            {
                var results = new List<object> { new { Username = username } };

                ResultUsernames.ItemsSource = results;
                DisplayMessage(UsernameStatusTextBlock, "Query successful.");
            }
            else
            {
                DisplayMessage(UsernameStatusTextBlock, "No username found or failed to fetch data.");
            }
        }

        // Event handler for querying by username
        private async void btnEmailQuery_Click(object sender, RoutedEventArgs e)
        {
            string token = txToken.Text;  // Get token from the TextBox
            string username = txUsername.Text; // Get username from the TextBox

            // Display message if username is not provided
            if (string.IsNullOrEmpty(username))
            {
                DisplayMessage(EmailsStatusTextBlock, "Please enter a username.");
                return;
            }

            // Check if "Ignore forks" checkbox is checked
            bool ignoreForks = isForksCheckBox.IsChecked ?? false;

            // Add token to headers if provided
            if (!string.IsNullOrEmpty(token))
            {
                HEADER["Authorization"] = $"token {token}";
            }

            // Get emails associated with the username
            List<(string Email, string Author)> emails = await GetEmailsByUsername(username, ignoreForks);
            if (emails != null && emails.Count > 0)
            {
                var emailResults = emails.Select(email => new { email.Email, email.Author }).ToList();
                ResultEmails.ItemsSource = emailResults;
            }
            else
            {
                DisplayMessage(EmailsStatusTextBlock, "No emails found or failed to fetch data.");
            }
        }


        private void dgEmails_DoubleClick(object sender, MouseButtonEventArgs e)
{
    if (ResultEmails.SelectedItem != null)
    {
        var email = (dynamic)ResultEmails.SelectedItem;

        bool success = false;
        int retries = 3;

        while (!success && retries > 0)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Clipboard.Clear(); // Clear the clipboard before setting text

                    
                    if (OpenClipboard(IntPtr.Zero))
                    {
                        string rowClipboardData = "Email: " + email.Email + "\nAuthor: " + email.Author;
                        Clipboard.SetText(rowClipboardData);
                        CloseClipboard();
                        success = true;

                        MessageBox.Show("Email copied to clipboard.", "Email Copied", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            }
            catch (Exception ex)
            {
                retries--;
                if (retries == 0)
                {
                    MessageBox.Show($"Failed to copy email to clipboard after multiple attempts: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    System.Threading.Thread.Sleep(100); // Delay between retries
                }
            }
        }
    }
}

        // These dll imports are required for clipboard operations
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();


        // Method to find username by email using GitHub API
        private async Task<string> FindUserNameByEmail(string email)
        {
            using HttpClient client = new HttpClient();
            var url = $"{API_URL}/search/users?q={email}";
            var result = await ApiCall(client, url);

            if (result != null && result is JsonObject obj && obj["total_count"]?.GetValue<int>() > 0)
            {
                var username = obj["items"]?[0]?["login"]?.ToString();
                return username;
            }

            return null;
        }

   
        private async Task<List<(string Email, string Author)>> GetEmailsByUsername(string username, bool ignoreForks)
        {
            var emails = new List<(string Email, string Author)>();

            using (var context = new AppDbContext())  // Initialize DbContext for database operations
            {
                // Check if there are any emails associated with this username in the database
                var storedEmails = context.EmailAuthors
                                           .Where(ea => ea.Author == username)
                                           .Select(ea => new { ea.Email, ea.Author })
                                           .ToList();

                // If we find records in the database, return them
                if (storedEmails.Any())
                {
                    EmailsStatusTextBlock.Text = "Data found in the database.";
                    return storedEmails.Select(e => (e.Email, e.Author)).ToList();
                }
            }

            // If not found in the database, call the API
            using HttpClient client = new HttpClient();
            var url = $"{API_URL}/users/{username}/repos?per_page=100";

            try
            {
                var result = await ApiCall(client, url);

                if (result is JsonArray repos)
                {
                    using (var context = new AppDbContext())  // Re-initialize DbContext for database operations
                    {
                        foreach (var repo in repos)
                        {
                            // Check if the repository is a fork, then skip it
                            bool isFork = repo["fork"]?.GetValue<bool>() ?? false;
                            if (ignoreForks && isFork)
                            {
                                continue;  // We are skipping this repository
                            }

                            var repoName = repo["name"]?.ToString();
                            EmailsStatusTextBlock.Text = $"Fetching emails from {repoName}...";
                            var commitsUrl = $"{API_URL}/repos/{username}/{repoName}/commits?per_page=100";

                            var commits = await ApiCall(client, commitsUrl);

                            if (commits is JsonArray commitArray)
                            {
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

                                            // Save to the database, with error catching
                                            try
                                            {
                                                var emailAuthor = new EmailAuthor
                                                {
                                                    Email = email,
                                                    Author = username  // We save the GitHub username associated with the repo
                                                };
                                                context.EmailAuthors.Add(emailAuthor);
                                            }
                                            catch (Exception ex)
                                            {
                                                DisplayMessage(EmailsStatusTextBlock, $"Error occurred while saving emails: {ex.Message}\n{ex.InnerException?.Message}");
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        try
                        {
                            await context.SaveChangesAsync();  // Save changes to the database
                        }
                        catch (Exception ex)
                        {
                            DisplayMessage(EmailsStatusTextBlock, $"Error occurred while saving emails: {ex.Message}\n{ex.InnerException?.Message}");
                        }
                    }

                    EmailsStatusTextBlock.Text = "Query successful from GitHub API.";
                }
            }
            catch (Exception ex)
            {
                DisplayMessage(EmailsStatusTextBlock, $"Error occurred while fetching emails: {ex.Message}");
            }

            return emails;
        }



        private async Task<JsonNode> ApiCall(HttpClient client, string url)
        {
            await Task.Delay(DELAY);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in HEADER)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(content);
        }

        // Display message in the status text block
        private void DisplayMessage(TextBlock statusTextBlock, string message)
        {
            statusTextBlock.Text = message;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (ResultEmails.ItemsSource != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Email,Author");

                foreach (var item in ResultEmails.ItemsSource)
                {
                    dynamic email = item;
                    sb.AppendLine($"{email.Email},{email.Author}");
                }

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    Title = "Export Emails to CSV",
                    FileName = "emails.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                    MessageBox.Show("Emails exported to CSV successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            }
        }


        // Social Account Finder 

        private SocialAccountFinderController socialAccountFinderController = new SocialAccountFinderController();
        private int Checked = 0;
        private int Results = 0;

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            if (string.IsNullOrEmpty(username) || username == "Enter username here...")
            {
                MessageBox.Show("Please enter a username to check.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SearchButton.IsEnabled = false;
            UsernameInput.IsEnabled = false;
            EmailsStatusTextBlock.Text = $"Checking username {username}";
            Checked = 0;
            Results = 0;
            SocialMediaResults.Items.Clear();  // Clear previous results

            Stopwatch stopwatch = Stopwatch.StartNew();

            await socialAccountFinderController.CheckWebsitesAsync(username,
                status => SocialAccountResult.Text = status,
                resultUrl => AddResult(resultUrl)
            );

            stopwatch.Stop();
            Results = SocialMediaResults.Items.Count;

            if (Results == 0)
            {
                SocialAccountResult.Text = $"No accounts found for {username}.";
            }
            else
            {
                SocialAccountResult.Text = $"Found {Results} accounts for {username} in {stopwatch.Elapsed.TotalSeconds} seconds.";
                MessageBox.Show($"Found {Results} accounts for {username} in {stopwatch.Elapsed.TotalSeconds} seconds.", "Search Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            SearchButton.IsEnabled = true;
            UsernameInput.IsEnabled = true;
        }

        private void AddResult(string url)
        {
            Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(url))
                {
                    MessageBox.Show("Profile URL is null or empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var account = new SocialAccount
                {
                    ProfileUrl = url
                };

                SocialMediaResults.Items.Add(account);
            });
        }

        private void OnFocusInput(object sender, RoutedEventArgs e)
        {
            UsernameInput.Text = "";
        }

        private void btnSocialFinderExport_Click(object sender, RoutedEventArgs e)
        {
            if (SocialMediaResults.Items.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Profile URL");

                foreach (SocialAccount account in SocialMediaResults.Items)
                {
                    sb.AppendLine(account.ProfileUrl);  // Only export the ProfileUrl
                }

                // File Save Dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    Title = "Export Profile URLs to CSV",
                    FileName = "profile_urls.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                    MessageBox.Show("Profile URLs exported to CSV successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No data available in the DataGrid.", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // End of Social Account Finder


        // Begin of Subdomain finder

        // Subdomain Finder API 
        private const string SUBFINDER_API_URL = "https://api.subdomain.center/?domain=";

        private SubdomainFinderController subdomainFinderController = new SubdomainFinderController();

        private async void DomainSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string domain = DomainInput.Text.Trim();
            if (string.IsNullOrEmpty(domain) || domain == "Enter domain here...")
            {
                MessageBox.Show("Please enter a domain to check.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DomainSearchBtn.IsEnabled = false;
            DomainInput.IsEnabled = false;

            // Clear DataGrid
            SubDomainResults.Items.Clear();

            try
            {
                // Use the controller to find subdomains
                var subdomains = await subdomainFinderController.FindSubdomainsAsync(domain);

                foreach (var subdomain in subdomains)
                {
                    SubDomainResults.Items.Add(subdomain);  // SubdomainUrl object
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DomainSearchBtn.IsEnabled = true;
                DomainInput.IsEnabled = true;

            }


            Subdomainresult.Text = "Query Successful";
        }

        private void btnSubdomainExport_Click(object sender, RoutedEventArgs e)
        {
            if (SubDomainResults.Items.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Profile URL");

                foreach (var item in SubDomainResults.Items)
                {
                    // Dynamically get the Subdomain property if it exists
                    var subdomainProperty = item.GetType().GetProperty("Subdomain")?.GetValue(item, null)?.ToString();
                    if (!string.IsNullOrEmpty(subdomainProperty))
                    {
                        sb.AppendLine(subdomainProperty);  // Only export the ProfileUrl
                    }
                }

                // File Save Dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    Title = "Export Subdomains to CSV",
                    FileName = "subdomains.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                    MessageBox.Show("Subdomains exported to CSV successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No data available in the DataGrid.", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // End of Subdomain finder
    }
}
