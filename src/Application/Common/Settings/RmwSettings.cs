using Application.Common.Abstractions;

namespace Application.Common.Settings;

public class RmwSettings : IRmwSettings
{
    public string ServiceName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public PollySettings PollySettings { get; set; } = new();
    public EcapService EcapServices { get; set; } = new();
    public EcapCredential EcapCredentials { get; set; } = new();
}