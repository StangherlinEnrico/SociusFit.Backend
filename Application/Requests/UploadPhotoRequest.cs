namespace Application.Requests;

public class UploadPhotoRequest
{
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
}