using Application.Common.Enums;
using Application.Common.Models;

namespace Application.Common.Abstractions;

public interface IStorageServiceAdapter
{
    Task<string> UploadFile(MemoryStream memoryStream, string fileName, string fileId, TokenType tokenType = TokenType.Anonymous);
    Task<string> GetFileUrl(string fileId, TokenType tokenType = TokenType.Anonymous);
    Task<Stream> GetFileStream(string fileId);
    Task<Stream> DownloadFile(string url);
    Task<List<FileResponse>> GetFiles(List<string> fileIds, TokenType tokenType = TokenType.Anonymous);
}