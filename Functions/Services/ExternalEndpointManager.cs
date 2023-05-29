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

    public async Task<string> Get(string hostname)
    {
        try
        {
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.GetAsync($"http://{hostname}");

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Display the response content
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            // Handle any errors that occurred during the request
            return "error";
        }

        return "error";
    }

    public async Task<string> Post(string hostname, string body)
    {
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.PostAsync($"http://{hostname}",content);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Display the response content
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            // Handle any errors that occurred during the request
            return "error";
        }

        return "error";
    }

    public async Task<string> Delete(string hostname)
    {
        try
        {
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.DeleteAsync($"http://{hostname}");

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Display the response content
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            // Handle any errors that occurred during the request
            return "error";
        }

        return "error";
    }

    public async Task<string> Patch(string hostname, string body)
    {
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            // Send GET request to the API
            HttpResponseMessage response = await _httpClient.PatchAsync($"http://{hostname}",content);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Display the response content
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            // Handle any errors that occurred during the request
            return "error";
        }

        return "error";
    }
}