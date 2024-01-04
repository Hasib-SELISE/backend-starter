namespace Application.Common.Abstractions;

public interface IPushNotificationClient
{
    Task NotifyToClientAsync<T>(T notification, string context, bool success, string errorMessage);

    Task NotifyToUserAsync<T>(
        T notification,
        string messageKey,
        IEnumerable<string> userIds,
        bool success,
        string errorMessage,
        string detailRoute = null,
        string responseKey = null,
        IEnumerable<string> dynamicMessageKey = null);

    Task NotifyAsync(object payload);
}