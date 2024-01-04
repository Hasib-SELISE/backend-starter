namespace Application.Common.Models;

public class StorageConfig
{
    public string BaseUrl { get; set; }
    public string Version { get; set; }
    public string PreSignedEndPoint { get; set; }
    public string GetFileEndPoint { get; set; }
    public string GetFilesEndPoint { get; set; }
}