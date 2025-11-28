using Application.Requests;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class UploadPhotoRequestValidator : AbstractValidator<UploadPhotoRequest>
{
    public UploadPhotoRequestValidator()
    {
        RuleFor(x => x.PhotoData)
            .NotEmpty().WithMessage("Photo data is required")
            .Must(data => data.Length <= ProfileConstants.MaxPhotoSizeInBytes)
            .WithMessage($"Photo size cannot exceed {ProfileConstants.MaxPhotoSizeInBytes / (1024 * 1024)}MB");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .Must(fileName =>
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                return ProfileConstants.AllowedPhotoExtensions.Contains(extension);
            })
            .WithMessage($"Invalid photo format. Allowed formats: {string.Join(", ", ProfileConstants.AllowedPhotoExtensions)}");
    }
}