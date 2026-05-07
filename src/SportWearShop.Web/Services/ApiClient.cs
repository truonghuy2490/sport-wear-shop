using System.Text;
using System.Text.Json;

namespace SportWearShop.Web.Services;

// SportWearShop.Web/Services/ApiClient.cs
/// <summary>
/// api client
/// </summary>
/// 
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // GET request
    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    // POST request
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
    }

    // PUT request
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
    }

    // DELETE request
    public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
