using System.Text.RegularExpressions;
using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Product.</b> One message with headers. The title becomes the subject.</summary>
public partial class EmailChannel(string from) : INotificationChannel
{
    public string Description => "One message with a From header and the title as the subject. No length limit.";
    public string RecipientFormat => "An email address";
    public string ExampleRecipient => "mona.adel@example.com";
    public decimal CostPerPart => 0.05m;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailAddress();

    public string? ValidateRecipient(string recipient) =>
        EmailAddress().IsMatch(recipient) ? null : "Not an email address.";

    public RenderedMessage Render(Notification n) =>
        new([$"From: {from}\nSubject: {n.Title}\n\n{n.Body}"], $"Sent from {from}. One message, whatever the length.");

    public async Task<string> DeliverAsync(string recipient, string part)
    {
        await Task.Delay(120);
        return TextParts.ProviderId("email");
    }
}
