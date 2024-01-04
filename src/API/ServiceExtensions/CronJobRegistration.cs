using Hangfire;
using Hangfire.MemoryStorage;

namespace API.ServiceExtensions;

public static class CronJobRegistration
{
    /// <summary>
    /// Register cron jobs
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCronJobs(this IServiceCollection services)
    {
        services.AddHangfire(opt => opt.UseMemoryStorage());
        services.AddHangfireServer();
        
        return services;
    }
}