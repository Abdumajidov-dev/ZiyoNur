namespace ZiyoNur.Domain.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<string> UploadImageAsync(Stream imageStream, string fileName, int? maxWidth = null, int? maxHeight = null);
    Task DeleteFileAsync(string fileUrl);
    Task<bool> FileExistsAsync(string fileUrl);
    string GetFileUrl(string fileName);
}
