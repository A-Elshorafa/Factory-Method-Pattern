using System.Text.RegularExpressions;
using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Product.</b> Always one part. Title and body are cut so they fit the banner on a phone.</summary>
public partial class PushChannel : INotificationChannel
{
    private const int MaxTitle = 50, MaxBody = 150;

    public string Description => $"One banner. The title is cut to {MaxTitle} chars and the body to {MaxBody}. Free.";
    public string RecipientFormat => "Device token: 20–200 letters, digits, '_', ':' or '-'";
    public string ExampleRecipient => "fcm_7Hq2LmX9pR4tVw8zKc3N";
    public decimal CostPerPart => 0m;

    [GeneratedRegex(@"^[A-Za-z0-9_:\-]{20,200}$")]
    private static partial Regex DeviceToken();

    public string? ValidateRecipient(string recipient) =>
        DeviceToken().IsMatch(recipient) ? null : "Not a device token (20–200 letters, digits, '_', ':' or '-').";

    public RenderedMessage Render(Notification n)
    {
        var title = TextParts.Truncate(n.Title, MaxTitle);
        var body = TextParts.Truncate(n.Body, MaxBody);

        var cut = new List<string>();
        if (title != n.Title) cut.Add($"title cut to {MaxTitle} chars");
        if (body != n.Body) cut.Add($"body cut to {MaxBody} chars");

        return new([$"{title}\n{body}"], cut.Count == 0 ? "Fits the banner as is." : $"Doesn't fit the banner: {string.Join(", ", cut)}.");
    }

    public async Task<string> DeliverAsync(string recipient, string part)
    {
        await Task.Delay(25);
        return TextParts.ProviderId("push");
    }
}
