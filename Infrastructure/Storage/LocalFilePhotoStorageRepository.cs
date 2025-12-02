using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Storage;

/// <summary>
/// TODO: Implementazione TEMPORANEA per sviluppo locale senza Azure Blob Storage.
/// Le foto vengono salvate su file system locale nella cartella wwwroot/uploads/photos/
/// 
/// IMPORTANTE: Questa implementazione NON è adatta per produzione!
/// - Le foto NON sono persistenti tra restart applicazione in alcuni deploy
/// - NON scala orizzontalmente (load balancing)
/// - Nessuna CDN o ottimizzazione distribuzione
/// 
/// Quando Azure Blob Storage è configurato:
/// 1. Sostituire questa classe con AzureBlobPhotoStorageRepository nel DI
/// 2. Rimuovere questa classe
/// </summary>
public class LocalFilePhotoStorageRepository : IPhotoStorageRepository
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalFilePhotoStorageRepository> _logger;

    public LocalFilePhotoStorageRepository(ILogger<LocalFilePhotoStorageRepository> logger)
    {
        _logger = logger;

        // Path locale: wwwroot/uploads/photos/
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "photos");

        // Crea directory se non esiste
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
            _logger.LogInformation("Created local photo storage directory: {Path}", _uploadPath);
        }

        // Base URL per accesso via HTTP (serve file statici da wwwroot)
        _baseUrl = "/uploads/photos";

        _logger.LogWarning(
            "Using LOCAL FILE storage for photos. This is NOT recommended for production. " +
            "TODO: Configure Azure Blob Storage for production use."
        );
    }

    public Task<string> UploadPhotoAsync(
        Guid userId,
        byte[] photoData,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            // Salva file su disco
            File.WriteAllBytes(filePath, photoData);

            // Ritorna URL relativo (serve da wwwroot)
            var photoUrl = $"{_baseUrl}/{uniqueFileName}";

            _logger.LogInformation(
                "Photo saved locally for user {UserId}: {FileName} ({Size} bytes)",
                userId,
                uniqueFileName,
                photoData.Length
            );

            return Task.FromResult(photoUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save photo locally for user {UserId}", userId);
            throw new PhotoUploadFailedException("Failed to save photo to local storage", ex);
        }
    }

    public Task<byte[]> GetPhotoAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(photoUrl);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Photo not found: {photoUrl}");
            }

            var photoData = File.ReadAllBytes(filePath);
            return Task.FromResult(photoData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve photo from local storage: {PhotoUrl}", photoUrl);
            throw new PhotoUploadFailedException($"Failed to retrieve photo: {photoUrl}", ex);
        }
    }

    public Task DeletePhotoAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(photoUrl);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted local photo: {PhotoUrl}", photoUrl);
            }
            else
            {
                _logger.LogWarning("Photo not found for deletion: {PhotoUrl}", photoUrl);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete photo from local storage: {PhotoUrl}", photoUrl);
            throw new PhotoUploadFailedException($"Failed to delete photo: {photoUrl}", ex);
        }
    }

    public Task<bool> PhotoExistsAsync(string photoUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(photoUrl);
            var filePath = Path.Combine(_uploadPath, fileName);
            return Task.FromResult(File.Exists(filePath));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}