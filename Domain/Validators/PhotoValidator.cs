using Domain.Constants;

namespace Domain.Validators;

public class PhotoValidator
{
    public ValidationResult Validate(byte[] photoData, string fileName)
    {
        var errors = new List<string>();

        if (photoData == null || photoData.Length == 0)
            errors.Add("Photo data is required");
        else if (photoData.Length > ProfileConstants.MaxPhotoSizeInBytes)
            errors.Add($"Photo size exceeds maximum allowed size of {ProfileConstants.MaxPhotoSizeInBytes / (1024 * 1024)}MB");

        if (string.IsNullOrWhiteSpace(fileName))
            errors.Add("File name is required");
        else
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!ProfileConstants.AllowedPhotoExtensions.Contains(extension))
                errors.Add($"Invalid photo format. Allowed formats: {string.Join(", ", ProfileConstants.AllowedPhotoExtensions)}");
        }

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    public ValidationResult ValidatePhotoUrl(string photoUrl)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(photoUrl))
            errors.Add("Photo URL is required");
        else if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
            errors.Add("Invalid photo URL format");
        else if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            errors.Add("Photo URL must use HTTP or HTTPS protocol");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}