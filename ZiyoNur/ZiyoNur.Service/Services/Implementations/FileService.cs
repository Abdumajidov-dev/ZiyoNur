using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZiyoNur.Service.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ZiyoNur.Service.Services.Implementations;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadPath;
    private readonly string _baseUrl;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _uploadPath = _configuration["FileStorage:UploadPath"] ?? "wwwroot/uploads";
        _baseUrl = _configuration["FileStorage:BaseUrl"] ?? "https://localhost:7000/uploads";

        // Create upload directory if it doesn't exist
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "uploads")
    {
        try
        {
            // Generate unique filename
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderPath = Path.Combine(_uploadPath, folder);
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Create folder if it doesn't exist
            Directory.CreateDirectory(folderPath);

            // Save file
            using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            var fileUrl = $"{_baseUrl}/{folder}/{uniqueFileName}";
            _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);

            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, int? maxWidth = 800, int? maxHeight = 600)
    {
        try
        {
            // Load and resize image
            using var image = await Image.LoadAsync(imageStream);

            if (maxWidth.HasValue && maxHeight.HasValue)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth.Value, maxHeight.Value),
                    Mode = ResizeMode.Max // Maintain aspect ratio
                }));
            }

            // Convert to JPEG for consistency and compression
            var fileExtension = ".jpg";
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderPath = Path.Combine(_uploadPath, "images");
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Create folder if it doesn't exist
            Directory.CreateDirectory(folderPath);

            // Save with JPEG encoder
            await image.SaveAsJpegAsync(filePath, new JpegEncoder
            {
                Quality = 85 // Good quality with compression
            });

            var fileUrl = $"{_baseUrl}/images/{uniqueFileName}";
            _logger.LogInformation("Image uploaded and resized successfully: {FileUrl}", fileUrl);

            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // Extract file path from URL
            var uri = new Uri(fileUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            var filePath = Path.Combine(_uploadPath, relativePath.Replace(_baseUrl, "").TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileUrl}", fileUrl);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        return File.Exists(filePath);
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_baseUrl}/{fileName}";
    }

    public async Task<byte[]> ResizeImageAsync(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        using var image = Image.Load(imageBytes);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(maxWidth, maxHeight),
            Mode = ResizeMode.Max
        }));

        using var output = new MemoryStream();
        await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = 85 });
        return output.ToArray();
    }
}