using System.Net;

namespace SportWearShop.Web.Infrastructure.Api;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string? ResponseContent { get; }

    public ApiException(
        HttpStatusCode statusCode,
        string? responseContent = null)
        : base($"API request failed with status code {(int)statusCode}")
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}