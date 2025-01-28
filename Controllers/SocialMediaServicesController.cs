using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OsintEyeWeb.Data;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class SocialMediaServicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public SocialMediaServicesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient("SocialMedia");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _context.SocialMediaServices.ToListAsync();
        return Ok(services);
    }

    [HttpGet("{serviceName}/{username}")]
    public async Task<IActionResult> CheckUsername(string serviceName, string username)
    {
        var service = await _context.SocialMediaServices
            .FirstOrDefaultAsync(s => s.Name.ToLower() == serviceName.ToLower());

        if (service == null)
            return NotFound($"Service '{serviceName}' not found");

        try
        {
            var url = service.UrlTemplate.Replace("{}", username);
            var response = await _httpClient.GetAsync(url);

            bool exists;
            if (service.ErrorType == "status_code")
            {
                exists = response.IsSuccessStatusCode;
            }
            else if (service.ErrorType == "message" && !string.IsNullOrEmpty(service.ErrorMessage))
            {
                var content = await response.Content.ReadAsStringAsync();
                exists = !content.Contains(service.ErrorMessage);
            }
            else
            {
                exists = false;
            }

            return Ok(new { 
                exists = exists,
                url = url,
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
