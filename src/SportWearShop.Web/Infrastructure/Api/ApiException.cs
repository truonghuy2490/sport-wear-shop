using System.Net;
using SportWearShop.Shared.ViewModels.ErrorResponseModels;

namespace SportWearShop.Web.Infrastructure.Api;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string? ResponseContent { get; }

    public ErrorResponseModel? ErrorResponse { get; }

    public ApiException(
        HttpStatusCode statusCode,
        string? responseContent = null,
        ErrorResponseModel? errorResponse = null)
        : base(errorResponse?.Message ?? $"API request failed with status code {(int)statusCode}")
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        ErrorResponse = errorResponse;
    }
}