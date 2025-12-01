namespace Domain.Constants;

public static class ProfileConstants
{
    public const int MinAge = 18;
    public const int MaxAge = 120;

    public const int MinBioLength = 10;
    public const int MaxBioLength = 500;

    public const int MinCityLength = 2;
    public const int MaxCityLength = 100;

    public const int MinSportsRequired = 1;
    public const int MaxSportsAllowed = 10;

    public const int MaxPhotoSizeInBytes = 5 * 1024 * 1024; // 5MB

    public const int MinMaxDistance = 5; // 5 km minimo
    public const int MaxMaxDistance = 100; // 100 km massimo
    public const int DefaultMaxDistance = 25; // 25 km di default

    public static readonly string[] AllowedPhotoExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
}