using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

public class AzureBlobPhotoStorageRepository : IPhotoStorageRepository
{
    private readonly BlobContainerClient _containerClient;
    private readonly AzureBlobStorageSettings _settings;

    public AzureBlobPhotoStorageRepository(IOptions<AzureBlobStorageSettings> settings)
    {
        _settings = settings.Value;

        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadPhotoAsync(
        Guid userId,
        byte[] photoData,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var blobName = $"{userId}/{Guid.NewGuid()}{extension}";

            var blobClient = _containerClient.GetBlobClient(blobName);

            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            using var stream = new MemoryStream(photoData);

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken
            );

            return string.IsNullOrWhiteSpace(_settings.BaseUrl)
                ? blobClient.Uri.ToString()
                : $"{_settings.BaseUrl.TrimEnd('/')}/{blobName}";
        }
        catch (Exception ex)
        {
            throw new PhotoUploadFailedException("Failed to upload photo to blob storage", ex);
        }
    }

    public async Task<byte[]> GetPhotoAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobName = ExtractBlobNameFromUrl(photoUrl);
            var blobClient = _containerClient.GetBlobClient(blobName);

            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream, cancellationToken);

            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            throw new PhotoUploadFailedException($"Failed to retrieve photo from blob storage: {photoUrl}", ex);
        }
    }

    public async Task DeletePhotoAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobName = ExtractBlobNameFromUrl(photoUrl);
            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PhotoUploadFailedException($"Failed to delete photo from blob storage: {photoUrl}", ex);
        }
    }

    public async Task<bool> PhotoExistsAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobName = ExtractBlobNameFromUrl(photoUrl);
            var blobClient = _containerClient.GetBlobClient(blobName);

            return await blobClient.ExistsAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    private string ExtractBlobNameFromUrl(string photoUrl)
    {
        var uri = new Uri(photoUrl);
        var containerPath = $"/{_settings.ContainerName}/";
        var path = uri.AbsolutePath;

        var startIndex = path.IndexOf(containerPath, StringComparison.OrdinalIgnoreCase);
        if (startIndex >= 0)
        {
            return path.Substring(startIndex + containerPath.Length);
        }

        return path.TrimStart('/');
    }
}