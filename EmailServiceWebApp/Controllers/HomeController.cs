using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmailServiceWebApp.Models;
using System.Text.Json;

namespace EmailServiceWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private const string SendApiURL = "/api/Email/send";
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(string recipientEmail)
    {
        var message = String.Empty;
        if (!string.IsNullOrEmpty(recipientEmail))
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + SendApiURL;
            var requestData = new { RecipientEmail = recipientEmail };
            var jsonRequest = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage? response = null;
            try
            {
                response = await client.PostAsync(apiUrl, content);
            }
            catch (HttpRequestException ex)
            {
                message = "Api is not running.";
                _logger.LogError(message);
            }

            if (response != null && response.IsSuccessStatusCode)
            {
                message = "Email sent.";
                _logger.LogInformation(message);
            }
            else if(response != null)
            {
                message = $"Failed to send email. Status code: {response.StatusCode}";
                _logger.LogError(message);
            }
        }
        else
        {
            message = "Recipient email cannot be empty.";
            _logger.LogWarning(message);
        }
        ViewBag.Message = message;
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
