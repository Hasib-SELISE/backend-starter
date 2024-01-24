using API.ServiceExtensions;
using Application;
using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Settings;
using Infrastructure;
using MediatR;
using Selise.Ecap.Infrastructure;
using System.Reflection;

var ecapWebApiPipelineBuilderOptions = new EcapWebApiPipelineBuilderOptions
{
    UseFileLogging = true,
    UseConsoleLogging = true,
    CommandLineArguments = args,
    UseAuditLoggerMiddleware = true,
    AddApplicationServices = AddApplicationServices,
    UseJwtBearerAuthentication = true,
    AddRequiredQueues = AddRequiredQueues,
};

void AddApplicationServices(IServiceCollection services, IAppSettings appSettings)
{
    var rmwSettings = new RmwSettings();

    services.AddSingleton<IRmwSettings>(rmwSettings);

    var provider = services.BuildServiceProvider();

    var configuration = (IConfiguration)provider.GetRequiredService(typeof(IConfiguration));
    configuration.Bind(rmwSettings);

    services.AddCoreInfrastructures(rmwSettings);
    services.RegisterApplicationCommandHandlers();
    services.RegisterApplicationQueryHandlers();
    services.RegisterApplicationServices();
    services.RegisterInfrastructureServices();
    services.RegisterCronJobs();
    services.RegisterApplicationValidators();
    services.RegisterApplicationEventHandlers();

    services.AddMediatR(Assembly.GetExecutingAssembly());

    EnableSwagger(services, appSettings);
}

IEnumerable<string> AddRequiredQueues(IAppSettings appSettings)
{
    return new[]
    {
        appSettings.EcapAuditLogQueueName,
    };
}

static void EnableSwagger(IServiceCollection services, IAppSettings appSettings)
{
    services.AddApiDocument(
        new EcapSwaggerOptions
        {
            Description = "[Application_Name] Business service",
            ApplicableEnvironments = new[]
            {
                "dev-az", "stg-az", "prod-az", "development",
            },
        },
        appSettings);
}

var app = EcapWebApiPipelineBuilder.BuildEcapWebApiPipeline(ecapWebApiPipelineBuilderOptions).Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;


app.Run();