using Selise.Ecap.Entities.PrimaryEntities.Delta;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Abstractions;

public interface IRmwSecurityContextProvider
{
    SecurityContext GetSecurityContext();
    Task<SecurityContext> GetSecurityContextAsync(string oauthBearerToken);
    Task<UserInfo> GetUserInfo();
}