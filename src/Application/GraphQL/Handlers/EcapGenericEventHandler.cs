using Application.Common.Abstractions;
using Application.GraphQL.Events;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Selise.Ecap.Entities.PrimaryEntities.Security;
using Selise.Ecap.Infrastructure;

namespace Application.GraphQL.Handlers;

public class EcapGenericEventHandler : IEventHandler<GenericEvent, CommandResponse>
{
    private readonly IRmwLogger<EcapGenericEventHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IRmwSecurityContextProvider _securityContextProvider;

    public EcapGenericEventHandler(
        IRmwLogger<EcapGenericEventHandler> logger,
        IConfiguration configuration,
        IRmwSecurityContextProvider securityContextProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _securityContextProvider = securityContextProvider;
    }
    
    public CommandResponse Handle(GenericEvent @event)
    {
        return HandleAsync(@event).Result;
    }

    public async Task<CommandResponse> HandleAsync(GenericEvent @event)
    {
        try
        {
            var securityContext = _securityContextProvider.GetSecurityContext();
            if (securityContext.TenantId == _configuration["TenantId"])
            {
                var payload = JsonConvert.DeserializeObject<User>(@event.JsonPayload);
                /*await _analyzerService.AnalyzeDataChangeAsync(payload);*/
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"EcapGenericEventHandler: {ex.Message}");
        }

        return new CommandResponse
        {
            HttpStatusCode = System.Net.HttpStatusCode.OK
        };
    }
}