using System.Net;
using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RecruitingMoms.Common.Constants;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class StorageServiceAdapter : IStorageServiceAdapter
    {

        private readonly IRestCommunicationClient _restService;
        private readonly IConfiguration _configuration;
        private readonly IAppSettings _appSettings;
        private readonly StorageConfig _storageConfig;

        public StorageServiceAdapter(IRestCommunicationClient restService,
            IConfiguration configuration, IAppSettings appSettings)
        {
            _restService = restService;
            _configuration = configuration;
            _appSettings = appSettings;
            _storageConfig = GetStorageConfig();
        }

        public async Task<string> UploadFile(MemoryStream memoryStream, string fileName, string fileId, TokenType tokenType = TokenType.Anonymous)
        {
            var response = new CommonResponseModel
            {
                ResponseMessage = "Error Occurred",
                HttpStatusCode = 500,
                Status = false
            };

            var getPreSignedUri = $"{_storageConfig.BaseUrl}{_storageConfig.Version}{_storageConfig.PreSignedEndPoint}";

            var requestBody = CreateRequestBody(fileName, fileId);

            var restResponse = await _restService.PostRequestAsync
                (new Uri(getPreSignedUri), requestBody, tokenType, 3);

            if (string.IsNullOrWhiteSpace(restResponse))
            {
                return string.Empty;
            }

            var responseFileId = await StoreFile(restResponse, memoryStream);

            if (!string.IsNullOrWhiteSpace(responseFileId))
            {
                response.ResponseMessage = "File Uploaded Successfully";
                response.HttpStatusCode = 200;
                response.Status = true;
                response.ResponseData = responseFileId;
            }

            return responseFileId;

        }

        public async Task<string> GetFileUrl(string fileId, TokenType tokenType = TokenType.Anonymous)
        {
            var fileUrl = string.Empty;

            var getFileUri = $"{_storageConfig.BaseUrl}{_storageConfig.Version}{_storageConfig.GetFileEndPoint}";
            var uriWithQueryParam = $"{getFileUri}?{RecruitingMomsServiceConstants.StorageGetFileByIdQueryParam}={fileId}";

            var restResponse = await _restService.GetRequestAsync(new Uri(uriWithQueryParam), tokenType, 3);

            if (restResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var storageResponse = JsonConvert.DeserializeObject(restResponse.ResponseData);
                fileUrl = storageResponse?.Url;
            }

            return fileUrl;
        }

        public async Task<List<FileResponse>> GetFiles(List<string> fileIds, TokenType tokenType = TokenType.Anonymous)
        {
            var storageResponse = new List<FileResponse>();

            var getFileUri = $"{_storageConfig.BaseUrl}{_storageConfig.Version}{_storageConfig.GetFilesEndPoint}";
            var payload = new { FileIds = fileIds };

            var restResponse = await _restService.PostRequestAsync(new Uri(getFileUri), payload, tokenType, 3);

            if (!string.IsNullOrWhiteSpace(restResponse))
            {
                storageResponse = JsonConvert.DeserializeObject<List<FileResponse>>(restResponse);
            }

            return storageResponse;
        }

        public async Task<Stream> GetFileStream(string fileId)
        {
            var streamData = Stream.Null;

            var fileUrl = await GetFileUrl(fileId);
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                streamData = await DownloadFile(fileUrl);
            }
            return streamData;
        }

        public async Task<Stream> DownloadFile(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return await _restService.DownloadFileStream(new Uri(url));
        }

        private async Task<string> StoreFile(string restResponse, MemoryStream memoryStream)
        {
            var getPreSignedUriResponse = JsonConvert.DeserializeObject<StorageResponse>(restResponse);

            var uploadUrl = getPreSignedUriResponse?.UploadUrl;
            var fileId = getPreSignedUriResponse?.FileId;

            if (string.IsNullOrWhiteSpace(uploadUrl) || string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }

            var headers = new Dictionary<string, string> { { "x-ms-blob-type", "BlockBlob" } };
            var uploadResponse = await _restService.PutRequestAsync(memoryStream, new Uri(uploadUrl), headers);

            return uploadResponse == HttpStatusCode.OK ? fileId : null;
        }

        private StorageConfig GetStorageConfig()
        {
            var storageConfig = new StorageConfig
            {
                BaseUrl = _configuration.GetSection("StorageBaseUrl").Value,
                Version = _configuration.GetSection("StorageVersion").Value,
                PreSignedEndPoint = _configuration.GetSection("StorageGetPreSignedUrlEndPoint").Value,
                GetFileEndPoint = _configuration.GetSection("StorageGetFileEndPoint").Value,
                GetFilesEndPoint = _configuration.GetSection("StorageGetFilesEndPoint").Value
            };

            if (string.IsNullOrWhiteSpace(storageConfig.BaseUrl))
            {
                storageConfig.BaseUrl = _appSettings.StorageServiceBaseUrl;
            }

            return storageConfig;
        }

        private static StorageRequestBody CreateRequestBody(string fileName, string fileId)
        {
            var tempMetaData = new Dictionary<string, MetaValue>
            {
                {"Title", new MetaValue {Type = "string", Value = fileName}},
                {"OriginalName", new MetaValue {Type = "string", Value = fileName}}
            };

            var serializedTempData = JsonConvert.SerializeObject(tempMetaData);

            var tempTag = new List<string> { "File" }.ToArray();
            var serializedTempTag = JsonConvert.SerializeObject(tempTag);

            var storageRequest = new StorageRequestBody
            {
                ItemId = fileId,
                Name = fileName,
                ParentDirectoryId = null,
                Tags = serializedTempTag,
                MetaData = serializedTempData
            };

            return storageRequest;
        }
    }