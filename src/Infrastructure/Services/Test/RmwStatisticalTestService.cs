using Application.Common.Abstractions;
using Application.Common.Models;
using Application.Services;
using Application.Test.Queries;
using MongoDB.Driver;
using Selise.Ecap.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Test;

public class RmwStatisticalTestService : IRmwStatisticalTestService
{
    private readonly IEcapMongoDbDataContextProvider _dataContextProvider;
    private readonly IRmwLogger<RmwStatisticalTestService> _logger;

    public RmwStatisticalTestService(IEcapMongoDbDataContextProvider dataContextProvider, IRmwLogger<RmwStatisticalTestService> logger)
    {
        _dataContextProvider = dataContextProvider;
        _logger = logger;
    }

    public async Task<DynamicQueryResponseModel> GetAnyDataAsync(GetDataQuery query)
    {
        var result = new DynamicQueryResponseModel();
        try
        {
            var collection = GetCollection(query).Aggregate();

            if (!string.IsNullOrEmpty(query.Match))
            {
                collection = collection.Match(query.Match);
            }
            var data = collection.Skip(query.PageNumber * query.PageSize)
               .Limit(query.PageSize)
               .ToList();

            return await Task.FromResult(result.HandleQuerySuccess(data, System.Net.HttpStatusCode.OK));

        }
        catch (Exception exception)
        {
            _logger.LogError($"GetAnyDataService -> GetAnyData \n Message: {exception.Message} \n StackTrace: {exception.StackTrace}", exception);
            return result.HandleQueryError(System.Net.HttpStatusCode.InternalServerError, "Exception in GetAnyData");
        }
    }

    private IMongoCollection<dynamic> GetCollection(GetDataQuery query)
    {
        return _dataContextProvider
            .GetTenantDataContext(query.TenantId)
            .GetCollection<dynamic>(query.CollectionName);
    }
}
