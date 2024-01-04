using Application.Common.Settings;

namespace Application.Common.Abstractions;

public interface IRmwSettings
{
    public string ServiceName { get; set; }
    public string TenantId { get; set; }
    public PollySettings PollySettings { get; set; }
    public EcapService EcapServices { get; set; }
    public EcapCredential EcapCredentials { get; set; }
}