
# OSINT Eye Documentation

### Overview
OSINT Eye is a desktop application built using WPF (Windows Presentation Foundation) in C#. The application provides functionality for querying GitHub repositories, finding user emails, social media account lookups, and subdomain enumeration. It interacts with GitHub’s API and utilizes a database to store email and author information for faster subsequent queries. The application also provides export capabilities for results to CSV files.

### Key Features:
- Query GitHub for usernames by email.
- Retrieve emails associated with a GitHub username and store them in a database.
- Copy email results to the clipboard.
- Search for social media accounts by username.
- Enumerate subdomains of a specified domain.
- Export results to CSV format.

### Installation
- Clone the repository to your local machine.
- Open the solution in Visual Studio.
- Build the solution to restore NuGet packages.
- Add-Migration InitialCreate and Update-Database in the Package Manager Console to create the database.
- Run the application.

### Components
### 1. **GitHub Username Finder (by Email)**

#### Functionality:
-   **FindUserNameByEmail**: This function queries GitHub’s API to search for users based on their email address.
-   **Event Handler (btnUsernameQuery_Click)**: A WPF button click event that triggers the email-to-username search. Displays results in a list.

#### Usage:
-   Input an email address in the `txEmail` TextBox.
-   Press the "Find Username" button.
-   The application will fetch the username associated with the email if available.

#### API Call:
-   The method `FindUserNameByEmail` sends a GET request to GitHub's API using an email as a search query parameter.

#### Example Code Snippet:
```
private async Task<string> FindUserNameByEmail(string email)
{
    using HttpClient client = new HttpClient();
    var url = $"{API_URL}/search/users?q={email}";
    var result = await ApiCall(client, url);
    // Process the response to extract the username.
}
```
### 2. **GitHub Email Finder (by Username)**

#### Functionality:
-   **GetEmailsByUsername**: This function fetches emails from GitHub repositories associated with a username. The results are stored in a database for future queries.
-   **Event Handler (btnEmailQuery_Click)**: This event is triggered when a user inputs a GitHub username and clicks the search button. The function retrieves email addresses from the user’s public repositories.

#### Database Storage:
-   Emails and associated authors are saved into a local database using Entity Framework Core, avoiding repeated API calls for the same username.

#### Example Code Snippet:
```
using (var context = new AppDbContext())
{
    var storedEmails = context.EmailAuthors
                               .Where(ea => ea.Author == username)
                               .Select(ea => new { ea.Email, ea.Author })
                               .ToList();
    // If emails are found in the database, return them.
}
```
### 3. **Email Copy to Clipboard**

#### Functionality:

-   Double-clicking an email in the `ResultEmails` DataGrid copies the selected email and author to the clipboard.
-   **Clipboard Operation**: The program interacts with Windows clipboard APIs (`user32.dll`) to handle clipboard operations.

#### Example Code Snippet:
```
Clipboard.SetText(rowClipboardData);
MessageBox.Show("Email copied to clipboard.", "Email Copied", MessageBoxButton.OK, MessageBoxImage.Information);
```
### 4. **Social Media Account Finder**

#### Functionality:
-   This feature checks various websites for social media accounts associated with the given username.
-   **SearchButton_Click**: Triggers the search process and updates the UI with found accounts.

#### Example Code Snippet:
```
await socialAccountFinderController.CheckWebsitesAsync(username,
    status => SocialAccountResult.Text = status,
    resultUrl => AddResult(resultUrl)
);
```
#### Result Handling:

-   Results are displayed in the `SocialMediaResults` DataGrid and can be exported to a CSV file.

### 5. **Subdomain Finder**

#### Functionality:

-   **Subdomain Finder**: Finds subdomains for a given domain using a third-party API.
-   **DomainSearchBtn_Click**: Initiates the query process for subdomains and populates the results in the `SubDomainResults` DataGrid.

#### Example Code Snippet:
```
var subdomains = await subdomainFinderController.FindSubdomainsAsync(domain);
foreach (var subdomain in subdomains)
{
    SubDomainResults.Items.Add(subdomain);
}
```
### 6. **Export to CSV**

#### Functionality:

-   Results from the Email Finder, Social Media Finder, and Subdomain Finder can be exported to CSV files.
-   **btnExport_Click**: Initiates the export process by building a CSV format string and saving it using a Save File dialog.

#### Example Code Snippet:
```
var saveFileDialog = new Microsoft.Win32.SaveFileDialog
{
    Filter = "CSV file (*.csv)|*.csv",
    Title = "Export Emails to CSV",
    FileName = "emails.csv"
};

```

### Helper Functions

#### **ApiCall Method**

-   Sends HTTP GET requests and processes the JSON response.
-   Implements rate-limiting with a delay to avoid exceeding GitHub API rate limits.

```
private async Task<JsonNode> ApiCall(HttpClient client, string url)
{
    await Task.Delay(DELAY);
    var response = await client.SendAsync(request);
    // Parse and return the response as JSON.
}
```
#### **DisplayMessage**
-   Updates a `TextBlock` UI element with a message to provide feedback on the operation’s success or failure.

```
private void DisplayMessage(TextBlock statusTextBlock, string message)
{
    statusTextBlock.Text = message;
}
```

## User Interface Components

1.  **TextBoxes**: For user inputs such as email, username, and domain.
2.  **Buttons**: Trigger various search and export functionalities.
3.  **DataGrids**: Display search results (Emails, Social Media Accounts, Subdomains).
4.  **TextBlocks**: For displaying status messages and results.

## External Libraries

-   **HttpClient**: To handle HTTP requests and interact with GitHub’s API.
-   **Entity Framework Core**: To handle database operations.
-   **Newtonsoft.Json**: For JSON parsing and processing.
-   **Windows Clipboard APIs (user32.dll)**: To manage clipboard operations.

## Error Handling

-   The program includes various try-catch blocks to handle potential exceptions, especially when interacting with APIs and the database.
-   Retry mechanisms are implemented when copying data to the clipboard to ensure stability.

## Conclusion

OSINT Eye is a powerful desktop tool for GitHub information retrieval, social media account searching, and subdomain enumeration. It provides a user-friendly interface and efficiently handles data with caching, rate-limiting, and error-handling mechanisms. Users can easily export their results for further use.