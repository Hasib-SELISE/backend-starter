using Application;
using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Settings;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selise.Ecap.Hosting.RabbitMq;
using Selise.Ecap.Infrastructure;
using WebHost.ServiceExtensions;

namespace WebHost;

static class Program
{
    static void Main(string[] args)
    {
        var rabbitMqHostBuilderOptions = new RabbitMqHostBuilderOptions
        {
            UseFileLogging = true,
            UseConsoleLogging = true,
            CommandLineArguments = args,
            AddApplicationServices = AddApplicationServices,
            AddAmpqMessageConsumerOptions = AmpqServiceRegistration.AddMessageConsumerOptions,
            AddRequiredQueues = AmpqServiceRegistration.AddRequiredQueues
        };

        var hostBuilder = RabbitMqHostBuilder
            .BuildRabbitMqHost(rabbitMqHostBuilderOptions)
            .UserStartupClass<ServiceStartup>();
        hostBuilder.Build().Run();
    }

    private static void AddApplicationServices(IServiceCollection serviceCollection, IAppSettings appSettings)
    {
        var rmwSettings = new RmwSettings();
        
        var provider = serviceCollection.BuildServiceProvider();
        serviceCollection.AddSingleton<IRmwSettings>(rmwSettings);
        
        var configuration = (IConfiguration)provider.GetRequiredService(typeof(IConfiguration));
        configuration.Bind(rmwSettings);
        
        serviceCollection.AddCoreInfrastructures(rmwSettings);
        serviceCollection.AddApplicationConfiguration(provider);
        serviceCollection.RegisterHostApplicationServices(configuration);
        serviceCollection.RegisterApplicationServices();
        serviceCollection.RegisterInfrastructureServices();
        serviceCollection.RegisterApplicationCommandHandlers();
        serviceCollection.RegisterApplicationValidators();
        serviceCollection.RegisterApplicationEventHandlers();
    }
}