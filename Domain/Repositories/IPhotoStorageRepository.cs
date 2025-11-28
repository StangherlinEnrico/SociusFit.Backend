namespace Domain.Repositories;

public interface IPhotoStorageRepository
{
    Task<string> UploadPhotoAsync(Guid userId, byte[] photoData, string fileName, CancellationToken cancellationToken = default);
    Task<byte[]> GetPhotoAsync(string photoUrl, CancellationToken cancellationToken = default);
    Task DeletePhotoAsync(string photoUrl, CancellationToken cancellationToken = default);
    Task<bool> PhotoExistsAsync(string photoUrl, CancellationToken cancellationToken = default);
}