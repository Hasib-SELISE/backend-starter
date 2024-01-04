using Selise.Ecap.Hosting.Application;
using Selise.Ecap.Hosting.Infrastructure;
using Selise.Ecap.Hosting.Middlewares;

namespace WebHost;

public class ServiceStartup : IStartup
{
    public void Configure(IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<RoutingMiddleware>();
        applicationBuilder.UseMiddleware<AuthorizationMiddleware>();
        applicationBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
        applicationBuilder.UseMiddleware<AuditLoggerMiddleware>();
        applicationBuilder.UseMiddleware<CommandHandlerMiddleware>();
    }
}