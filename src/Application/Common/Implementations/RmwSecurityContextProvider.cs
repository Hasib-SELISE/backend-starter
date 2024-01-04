using Application.Common.Abstractions;
using Selise.Ecap.Entities.PrimaryEntities.Delta;
using Selise.Ecap.Entities.PrimaryEntities.PlatformDataService;
using Selise.Ecap.GraphQL.Entity;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class RmwSecurityContextProvider : IRmwSecurityContextProvider
{
    private readonly IRmwRepository _repo;
    private readonly ISecurityContextProvider _securityContextProvider;

    public RmwSecurityContextProvider(
        IRmwRepository repo,
        ISecurityContextProvider securityContextProvider)
    {
        _repo = repo;
        _securityContextProvider = securityContextProvider;
    }
    
    public SecurityContext GetSecurityContext()
    {
        return _securityContextProvider.GetSecurityContext();
    }

    public Task<SecurityContext> GetSecurityContextAsync(string oauthBearerToken)
    {
        return _securityContextProvider.GetSecurityContextAsync(oauthBearerToken);
    }

    public async Task<UserInfo> GetUserInfo()
    {
        var securityContext = _securityContextProvider.GetSecurityContext();

        var connection = await _repo.GetItemAsync<Connection>(x => x.ParentEntityID == securityContext.UserId);
        if (connection == null)
        {
            return null;
        }

        var person = await _repo.GetItemAsync<Person>(x => x.ItemId == connection.ChildEntityID);
        if (person == null)
        {
            return null;
        }
        return new UserInfo
        {
            Avatar = person.ProfileImageId,
            Name = person.DisplayName,
            PersonId = person.ItemId,
            UserInfoId = securityContext.UserId
        };
    }
}