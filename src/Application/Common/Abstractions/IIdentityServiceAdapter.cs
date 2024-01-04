using Application.Common.Enums;
using Application.Common.Models.ResponseModels;

namespace Application.Common.Abstractions;

public interface IIdentityServiceAdapter
{
    Task<IdentityTokenResponse> GetTokenAsync(EcapCredentialType credentialType);
}