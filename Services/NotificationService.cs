using NotificationFactoryMethod.Models;
using NotificationFactoryMethod.Notifications;

namespace NotificationFactoryMethod.Services;

/// <summary>
/// <b>Client.</b> Picks the dispatcher for the requested channel and calls it through the
/// <see cref="NotificationDispatcher"/> base type. It never creates or names a concrete channel.
/// </summary>
public class NotificationService(IEnumerable<NotificationDispatcher> dispatchers, NotificationOutbox outbox)
{
    private readonly Dictionary<NotificationChannel, NotificationDispatcher> _dispatchers = dispatchers.ToDictionary(d => d.Channel);

    public bool Supports(NotificationChannel channel) => _dispatchers.ContainsKey(channel);

    public List<ChannelInfo> Channels() => _dispatchers.Values.Select(d => d.Describe()).ToList();

    public async Task<DispatchResult> SendAsync(SendNotificationRequest request)
    {
        var result = await _dispatchers[request.Channel].DispatchAsync(request.Recipient.Trim(), new Notification(request.Title, request.Body));
        outbox.Add(result);
        return result;
    }

    public PreviewResult Preview(PreviewRequest request)
    {
        var notification = new Notification(request.Title, request.Body);
        var channels = request.Channels.Count > 0 ? request.Channels.Distinct() : _dispatchers.Keys;
        return new(request.Title.Length, request.Body.Length, channels.Select(c => _dispatchers[c].Preview(notification)).ToList());
    }
}
