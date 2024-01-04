using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationConfigurationExtension
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services,
        IServiceProvider serviceProvider)
    {
        return services;
    }
}