using Application.Common.Enums;

namespace Application.Common.Settings;

public class Credential
{
    public EcapCredentialType CredentialType { get; set; }
    public string OriginUrl { get; set; } = string.Empty;
    public List<CredentialPayload> CredentialPayloads { get; set; } = new();
}