using Microsoft.AspNetCore.Http;

namespace MottuBackend.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<byte[]?> GetFileAsync(string filePath);
}
