using System.Net;
using System.Text;
using Functions.Services.Interfaces;

namespace Functions.Services;

public class ExternalEndpointManager : IExternalEndpointManager
{
    private readonly ILogger<ExternalEndpointManager> _logger;
    private readonly HttpClient _httpClient;
    
    public ExternalEndpointManager(ILogger<ExternalEndpointManager> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> Get(string hostname)
    {
        try
        {
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.GetAsync($"http://{hostname}");

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Display the response content
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            // Handle any errors that occurred during the request
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<HttpResponseMessage> Post(string hostname, string body)
    {
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.PostAsync($"http://{hostname}",content);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Display the response content
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            // Handle any errors that occurred during the request
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<HttpResponseMessage> Delete(string hostname)
    {
        try
        {
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.DeleteAsync($"http://{hostname}");

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Display the response content
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            // Handle any errors that occurred during the request
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<HttpResponseMessage> Patch(string hostname, string body)
    {
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.PatchAsync($"http://{hostname}",content);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Display the response content
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            // Handle any errors that occurred during the request
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<HttpResponseMessage> Put(string hostname, string body)
    {
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.PutAsync($"http://{hostname}",content);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Display the response content
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            // Handle any errors that occurred during the request
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}