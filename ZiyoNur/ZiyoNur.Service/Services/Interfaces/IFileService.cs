namespace ZiyoNur.Service.Services.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "uploads");
    Task<string> UploadImageAsync(Stream imageStream, string fileName, int? maxWidth = 800, int? maxHeight = 600);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<bool> FileExistsAsync(string fileName);
    string GetFileUrl(string fileName);
    Task<byte[]> ResizeImageAsync(byte[] imageBytes, int maxWidth, int maxHeight);
}
