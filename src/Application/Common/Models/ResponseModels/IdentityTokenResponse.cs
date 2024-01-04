using Newtonsoft.Json;

namespace Application.Common.Models.ResponseModels;

public class IdentityTokenResponse
{
    [JsonProperty("scope")]
    public string Scope { get; set; }
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    [JsonProperty("expires_in")]
    public int? ExpiresIn { get; set; }
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonProperty("ip_address")]
    public string IpAddress { get; set; }
    public DateTime TokenInitiateTime { get; set; }
}