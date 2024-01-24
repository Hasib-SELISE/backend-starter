using Application.Common.Abstractions;
using Application.Common.Implementations;
using Application.Services;
using Infrastructure.Services.Event;
using Infrastructure.Services.Test;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IStorageServiceAdapter, StorageServiceAdapter>();
        services.AddTransient<IRmwStatisticalTestService, RmwStatisticalTestService>();
        services.AddTransient<IEventQueryService, EventQueryService>();
        return services;
    }
}