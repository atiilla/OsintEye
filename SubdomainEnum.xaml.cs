using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class SubdomainEnum : ContentPage
{
    private const string SUBFINDER_API_URL = "https://api.subdomain.center/?domain=";
    public ObservableCollection<SubdomainResult> DomainResults { get; set; } = new ObservableCollection<SubdomainResult>();

    public SubdomainEnum()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnDomainEnumerationClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(DomainEntry.Text))
        {
            await DisplayAlert("Error", "Domain is required", "OK");
            return;
        }

        try
        {
            DomainResults.Clear();
            var url = $"{SUBFINDER_API_URL}{DomainEntry.Text}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var subdomainList = JsonSerializer.Deserialize<List<string>>(content);

                    foreach (var subdomain in subdomainList)
                    {
                        DomainResults.Add(new SubdomainResult 
                        { 
                            Subdomain = subdomain,
                            Details = "Found via Subfinder API"
                        });
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to retrieve subdomains", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred while fetching subdomains", "OK");
        }
    }
}

public class SubdomainResult
{
    public string Subdomain { get; set; }
    public string Details { get; set; }
}