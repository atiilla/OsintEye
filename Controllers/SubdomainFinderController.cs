using Microsoft.AspNetCore.Mvc;
using OsintEyeWeb.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace OsintEyeWeb.Controllers
{
    public class SubdomainFinderController : Controller
    {
        private readonly ILogger<SubdomainFinderController> _logger;
        private const string SUBFINDER_API_URL = "https://api.subdomain.center/?domain=";

        public SubdomainFinderController(ILogger<SubdomainFinderController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FindSubdomains(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                ModelState.AddModelError("", "Domain is required.");
                return View("Index");
            }

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
                        var subdomainList = JsonSerializer.Deserialize<List<string>>(content);

                        foreach (var subdomain in subdomainList)
                        {
                            subdomains.Add(new SubdomainUrl { Subdomain = subdomain });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to retrieve subdomains for domain {Domain}. StatusCode: {StatusCode}", domain, response.StatusCode);
                        ModelState.AddModelError("", "Failed to retrieve subdomains.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching subdomains for {Domain}.", domain);
                ModelState.AddModelError("", "An error occurred while fetching subdomains.");
            }

            ViewBag.Subdomains = subdomains;
            return View("Results");
        }
    }
}
