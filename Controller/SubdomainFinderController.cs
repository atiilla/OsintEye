using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GitRecon.Models;

namespace GitRecon.Controllers
{
    public class SubdomainFinderController
    {
        // Subdomain Finder API 
        private const string SUBFINDER_API_URL = "https://api.subdomain.center/?domain=";

        public async Task<List<SubdomainUrl>> FindSubdomainsAsync(string domain)
        {
            var subdomains = new List<SubdomainUrl>();

            try
            {
                var url = $"{SUBFINDER_API_URL}{domain}";
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var subdomainList = JsonConvert.DeserializeObject<List<string>>(content);

                        // Convert to List<SubdomainUrl>
                        foreach (var subdomain in subdomainList)
                        {
                            subdomains.Add(new SubdomainUrl { Subdomain = subdomain });
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve data from the API.");
                    }
                }
            }
            catch (Exception ex)
            {
                // You can log the exception or handle it as needed
                throw new Exception($"Error occurred while fetching subdomains: {ex.Message}");
            }

            return subdomains;
        }
    }
}
