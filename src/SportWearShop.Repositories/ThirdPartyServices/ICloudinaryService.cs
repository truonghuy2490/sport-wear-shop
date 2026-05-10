using Microsoft.AspNetCore.Http;

namespace SportWearShop.Repositories.ThirdPartyServices;

public interface ICloudinaryService
{
    Task<string> UploadFileAsync(
        IFormFile file,
        string? folder = null,
        CancellationToken cancellationToken = default);

    Task<List<string>> UploadFilesAsync(
        List<IFormFile> files,
        string? folder = null,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string imageUrl,
        CancellationToken cancellationToken = default);
}