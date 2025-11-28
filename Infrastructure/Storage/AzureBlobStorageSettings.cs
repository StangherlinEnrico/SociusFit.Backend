namespace Infrastructure.Storage;

public class AzureBlobStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "profile-photos";
    public string BaseUrl { get; set; } = string.Empty;
}