using Microsoft.Extensions.Configuration;

namespace Application.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsNativeTenant(
            this IConfiguration configuration,
            string tenantId)
        {
            return configuration["TenantId"] == tenantId;
        }
    }
}
