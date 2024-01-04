using Application.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Selise.Ecap.Entities.PrimaryEntities.Security;
using Selise.Ecap.GraphQL.Entity;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class CustomSecurityContextProvider : ICustomSecurityContextProvider
{
    private readonly IMongoDatabase userDatabase;
    private readonly IMongoDatabase tenantDatabase;
    private const string TenantId = "986A938D-81CA-4D87-8DAC-7AB1470BBC48";
    private readonly IRmwLogger<CustomSecurityContextProvider> logger;
    private readonly IConfiguration config;
    private readonly string defaultAdminEmail;
    
    public CustomSecurityContextProvider(IEcapMongoDbDataContextProvider dataContextProvider, IAppSettings appSettings,
        IRmwLogger<CustomSecurityContextProvider> logger, IConfiguration config)
    {
        userDatabase = dataContextProvider.GetTenantDataContext(TenantId);
        var client = new MongoClient(appSettings.TenantRegistrationDatabaseConnectionString);
        tenantDatabase = client.GetDatabase("TenantRegistrations");
        this.logger = logger;
        this.config = config;
        defaultAdminEmail = this.config.GetSection("DefaultAdminEmail").Value;
    }

    public SecurityContext GetSecurityContext(string email = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            email = defaultAdminEmail;

        var userdb = userDatabase.GetCollection<User>("Users");
        var user = userdb.Find(x => x.Email == email).FirstOrDefault();
        var siteDb = tenantDatabase.GetCollection<Site>("Sites");
        var site = siteDb.Find(x => x.TenantId == TenantId).FirstOrDefault();
        var securityContext = new SecurityContext
        (
            email: user.Email,
            language: user.Language,
            requestOrigin: site.HostName,
            phoneNumber: string.IsNullOrWhiteSpace(user.PhoneNumber) ? "no-phone" : user.PhoneNumber,
            roles: user.Roles,
            sessionId: $"ecap-{Guid.NewGuid().ToString()}",
            siteId: site.ItemId,
            siteName: site.Name,
            tenantId: user.TenantId,
            displayName: user.DisplayName,
            userId: user.ItemId,
            isUserAuthenticated: true,
            userName: user.ItemId,
            hasDynamicRoles: false,
            userAutoExpire: user.AutoExpire,
            userExpireOn: DateTime.MinValue,
            userPrefferedLanguage: string.Empty,
            isAuthenticated: true,
            oauthBearerToken: string.Empty,
            requestUri: new Uri("about:blank"),
            serviceVersion: string.Empty,
            tokenHijackingProtectionHash: string.Empty,
            postLogOutHandlerDataKey: string.Empty,
            organizationId: string.Empty
        );

        logger.LogInformation("SecurityContext generated.");

        return securityContext;
    }
}