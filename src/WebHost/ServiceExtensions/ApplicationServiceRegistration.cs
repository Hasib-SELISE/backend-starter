using Application.Common.Abstractions;
using Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebHost.ServiceExtensions;

public static class ApplicationServiceRegistration
{
    public static void RegisterHostApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationSettings = new RmwSettings();
        configuration.Bind(applicationSettings);

        services.AddSingleton<IRmwSettings>(applicationSettings);
    }
}