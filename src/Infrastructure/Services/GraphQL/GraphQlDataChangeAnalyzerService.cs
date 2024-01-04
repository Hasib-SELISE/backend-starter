using Application.Common.Abstractions;
using Domain.Repositories;
using Selise.Ecap.Entities.PrimaryEntities.Security;
using Selise.Ecap.Infrastructure;

namespace Infrastructure.Services.GraphQL;

public class GraphQlDataChangeAnalyzerService
{
    private readonly IServiceClient _serviceClient;
    private readonly IDbRepository<GraphQlDataChangeAnalyzerService> _repository;
    private readonly IRmwLogger<GraphQlDataChangeAnalyzerService> _logger;

    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="serviceClient"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public GraphQlDataChangeAnalyzerService(
        IServiceClient serviceClient,
        IDbRepository<GraphQlDataChangeAnalyzerService> repository,
        IRmwLogger<GraphQlDataChangeAnalyzerService> logger)
    {
        _serviceClient = serviceClient;
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> AnalyzeDataChangeAsync(User payload)
    {
        return true;
    }
}