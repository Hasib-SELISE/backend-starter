using Application.Common.Abstractions;
using Application.Common.DefaultValueInjection;
using Application.Common.Enums;
using Application.Common.Handlers;
using Application.Common.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Selise.Ecap.Infrastructure;

namespace Application.Common;

public static class CoreServiceCollectionRegistration
{
    public static IServiceCollection AddCoreInfrastructures(this IServiceCollection services, IRmwSettings rmwSettings)
    {
        services.AddTransient<TokenDelegatingHandler>();
        services.AddSingleton<IHttpRequestGateway, HttpRequestGateway>();
        services.AddTransient<IRmwRepository, RmwRepository>();
        services.AddTransient<ICustomSecurityContextProvider, CustomSecurityContextProvider>();
        services.AddTransient<CommandHandler>();
        services.AddTransient<ICommandDispatcher, RmwCommandDispatcher>();
        services.AddTransient<IMessageDispatcher, MessageDispatcher>();
        services.AddTransient<IQueryDispatcher, QueryDispatcher>();
        services.AddTransient<IRowLevelSecurityInjection, RowLevelSecurityInjection>();
        services.AddTransient<IDefaultValueInjection, DefaultValueInjection.DefaultValueInjection>();
        services.AddTransient<IRmwSecurityContextProvider, RmwSecurityContextProvider>();
        services.AddSingleton(typeof(IRmwLogger<>), typeof(RmwLogger<>));
        services.AddTransient<RoleInterpreter>();
        
        services.AddHttpClient<IIdentityServiceAdapter, IdentityServiceAdapter>(client =>
            {
                client.BaseAddress = new Uri((rmwSettings
                        .EcapServices
                        .Services
                        .FindLast(x => x.ServiceName == EcapServiceType.Identity))
                    .ServiceUrl);
            });
        
        services.AddSingleton<IUamServiceAdapter, UamServiceAdapter>();
        services.AddHttpClient<IUamServiceAdapter, UamServiceAdapter>(client =>
        {
            client.BaseAddress = new Uri(rmwSettings
                .EcapServices
                .Services
                .FindLast(x => x.ServiceName == EcapServiceType.Uam)
                .ServiceUrl);
        });
        services.AddTransient<RoleInterpreter>();
        services.AddSingleton<HttpRequestHelper>();
        services.AddTransient<HttpClient>();
        services.AddTransient<IMongoDbCollectionProvider, MongoDbCollectionProvider>();
        services.AddTransient<IMongoQueryBuilder, MongoQueryBuilder>();
        services.AddTransient<IRestCommunicationClient, RestCommunicationClient>();
        services.AddTransient<IPushNotificationClient, PushNotificationClient>();
        services.AddTransient<IMailServiceClient, MailServiceClient>();
        services.AddTransient<ISequenceNumberServiceAdapter, SequenceNumberServiceAdapter>();

        services.AddHttpClient<IMailServiceClient, MailServiceClient>(client =>
        {
            client.BaseAddress = new Uri(rmwSettings
                .EcapServices
                .Services
                .FindLast(x => x.ServiceName == EcapServiceType.Mail)
                .ServiceUrl);
        });

        return services;
    }
}