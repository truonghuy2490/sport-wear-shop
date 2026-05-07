using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
namespace SportWearShop.Repositories.ThirdPartyServices.Implementations;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _configuration;

    public CloudinaryService(IConfiguration configuration)
    {
        _configuration = configuration;

        var cloudName = _configuration["Cloudinary:CloudName"];
        var apiKey = _configuration["Cloudinary:ApiKey"];
        var apiSecret = _configuration["Cloudinary:ApiSecret"];

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(
        IFormFile file,
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Image file is required.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Only JPG, JPEG, PNG and WEBP files are allowed.");

        try
        {
            var defaultFolder = _configuration["Cloudinary:ProductImageFolder"];

            var uploadFolder = string.IsNullOrWhiteSpace(folder)
                ? defaultFolder
                : folder;

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}";

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = uploadFolder,
                PublicId = fileName,
                UseFilename = false,
                UniqueFilename = false,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Error uploading file: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file: {ex.Message}", ex);
        }
    }

    public async Task<List<string>> UploadFilesAsync(
        List<IFormFile> files,
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        if (files == null || files.Count == 0)
            throw new Exception("Image files are required.");

        var uploadTasks = new ConcurrentBag<Task<string>>();

        foreach (var file in files)
        {
            uploadTasks.Add(UploadFileAsync(file, folder, cancellationToken));
        }

        var results = await Task.WhenAll(uploadTasks);

        return results.ToList();
    }

    public async Task DeleteFileAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        var publicId = GetPublicIdFromUrl(imageUrl);

        if (string.IsNullOrWhiteSpace(publicId))
            throw new Exception("PublicId is required.");

        try
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
                throw new Exception($"Error deleting file: {result.Error.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file: {ex.Message}", ex);
        }
    }

    private static string GetPublicIdFromUrl(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var path = uri.AbsolutePath;

        // Example:
        // /demo/image/upload/v1234567890/sport-wear-shop/products/abc.png

        var uploadIndex = path.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);

        if (uploadIndex == -1)
            throw new Exception("Invalid Cloudinary URL.");

        var publicIdWithExtension = path[(uploadIndex + "/upload/".Length)..];

        var parts = publicIdWithExtension.Split('/').ToList();

        if (parts.Count > 0 && parts[0].StartsWith("v"))
            parts.RemoveAt(0);

        var publicId = string.Join("/", parts);

        return Path.ChangeExtension(publicId, null);
    }
}