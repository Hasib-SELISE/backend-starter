namespace Application.Common.Settings;

public class PollySettings
{
    public string MaxRetry { get; set; } = string.Empty;
    public string AllowedErrorCountBeforeCircuitBreaks { get; set; } = String.Empty;
}