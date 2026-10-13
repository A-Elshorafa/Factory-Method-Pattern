using System.Text.RegularExpressions;
using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Product.</b> Bold title using WhatsApp's <c>*…*</c> markup. Splits at 4096 chars, billed per message.</summary>
public partial class WhatsAppChannel(string businessNumber) : INotificationChannel
{
    private const int MaxMessage = 4096;

    public string Description => $"Title in bold. Split into messages of up to {MaxMessage} chars, billed per message.";
    public string RecipientFormat => "Egyptian mobile number in international format only: +201XXXXXXXXX";
    public string ExampleRecipient => "+201012345678";
    public decimal CostPerPart => 0.45m;

    [GeneratedRegex(@"^\+201[0125]\d{8}$")]
    private static partial Regex InternationalEgyptianMobile();

    public string? ValidateRecipient(string recipient) =>
        InternationalEgyptianMobile().IsMatch(recipient) ? null : "WhatsApp needs the international format: +201XXXXXXXXX.";

    public RenderedMessage Render(Notification n)
    {
        var parts = TextParts.Split($"*{n.Title}*\n{n.Body}", MaxMessage, r => r.Utf16SequenceLength);
        return new(parts, $"From {businessNumber}. {(parts.Count == 1 ? "One message" : $"{parts.Count} messages")}.");
    }

    public async Task<string> DeliverAsync(string recipient, string part)
    {
        await Task.Delay(90);
        return TextParts.ProviderId("wa");
    }
}
