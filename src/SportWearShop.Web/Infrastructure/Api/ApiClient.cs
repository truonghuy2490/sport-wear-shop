using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SportWearShop.Web.Infrastructure.Api;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        return await HandleResponseAsync<T>(response, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            endpoint,
            data,
            _jsonOptions,
            cancellationToken);

        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            endpoint,
            data,
            _jsonOptions,
            cancellationToken);

        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new ApiException(
                response.StatusCode,
                errorContent);
        }
    }

    private async Task<T?> HandleResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new ApiException(
                response.StatusCode,
                errorContent);
        }

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await response.Content.ReadFromJsonAsync<T>(
            _jsonOptions,
            cancellationToken);
    }
}