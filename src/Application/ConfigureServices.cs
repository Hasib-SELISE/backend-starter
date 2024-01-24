using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.Profiles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Selise.Ecap.Hosting.Infrastructure;
using Selise.Ecap.Infrastructure;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CreateEventCommandProfile).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }

    public static IServiceCollection RegisterApplicationCommandHandlers(this IServiceCollection services)
    {
        services.RegisterCollection(typeof(ICommandHandler<,>), new List<Assembly>
        {
           /* typeof(CreateJobCommandHandler).Assembly,
            typeof(CreateCheckoutSessionCommandHandler).Assembly,*/
        });
        return services;
    }

    public static IServiceCollection RegisterApplicationQueryHandlers(this IServiceCollection services)
    {
        services.RegisterCollection(typeof(IQueryHandler<,>), new List<Assembly>
        {
            /*typeof(GetSubscriptionBalanceQuery).Assembly,*/
        });
        return services;
    }

    public static IServiceCollection RegisterApplicationValidators(this IServiceCollection services)
    {
        services.RegisterCollection(typeof(IValidator<>), new List<Assembly>
        {
            /*typeof(CreateCheckoutSessionCommandValidator).Assembly,*/
        });
        return services;
    }

    public static IServiceCollection RegisterApplicationEventHandlers(this IServiceCollection services)
    {
        services.RegisterCollection(typeof(IEventHandler<,>), new List<Assembly>
        {
            //typeof(PdfsFromHtmlCreatedBulkEventHandler).Assembly
        });

        return services;
    }
}