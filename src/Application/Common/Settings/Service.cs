using Application.Common.Enums;

namespace Application.Common.Settings;

public class Service
{
    public EcapServiceType ServiceName { get; set; }
    public string ServiceUrl { get; set; }
    public Dictionary<string,string> ServiceEndpoint { get; set; }
    public string ServiceVersion { get; set; }
    public string AccessKey { get; set; }
    public Service()
    {
        ServiceEndpoint = new Dictionary<string, string>();
    }
}