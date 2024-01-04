using Selise.Ecap.Infrastructure;

namespace Application.Common.Abstractions;

public interface ICustomSecurityContextProvider
{
    SecurityContext GetSecurityContext(string email = null);
}