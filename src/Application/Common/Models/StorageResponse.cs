﻿namespace Application.Common.Models;

public class StorageResponse
{
    public string UploadUrl { get; set; }
    public string FileId { get; set; }
    public int StatusCode { get; set; }
    public string RequestUri { get; set; }
    public string ExternalError { get; set; }
    public int HttpStatusCode { get; set; }
}