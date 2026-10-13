using System.Text;
using System.Text.RegularExpressions;
using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary>
/// <b>Concrete Product.</b> Splits the text into segments and bills each one. Text that fits the GSM-7 alphabet
/// gets 160 chars in one SMS or 153 per segment. Anything else (Arabic, emoji) is sent as UCS-2: 70, or 67 per segment.
/// </summary>
public partial class SmsChannel(string senderId) : INotificationChannel
{
    private const string Gsm7Basic =
        "@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !\"#¤%&'()*+,-./0123456789:;<=>?¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà";

    // These take an escape character first, so they count as two.
    private const string Gsm7Extension = "^{}\\[~]|€\f";

    public string Description => "Billed per segment. GSM-7 text: 160 chars, or 153 per segment. Arabic or emoji: 70, or 67 per segment.";
    public string RecipientFormat => "Egyptian mobile number, local or international: 01XXXXXXXXX or +201XXXXXXXXX";
    public string ExampleRecipient => "+201012345678";
    public decimal CostPerPart => 0.30m;

    [GeneratedRegex(@"^(\+20|0)1[0125]\d{8}$")]
    private static partial Regex EgyptianMobile();

    public string? ValidateRecipient(string recipient) =>
        EgyptianMobile().IsMatch(recipient) ? null : "Not an Egyptian mobile number (01XXXXXXXXX or +201XXXXXXXXX).";

    public RenderedMessage Render(Notification n)
    {
        var text = $"{n.Title}\n{n.Body}";
        var gsm7 = text.All(c => Gsm7Basic.Contains(c) || Gsm7Extension.Contains(c));

        var (single, segment, encoding) = gsm7 ? (160, 153, "GSM-7") : (70, 67, "UCS-2, because the text has characters outside GSM-7");
        Func<Rune, int> cost = gsm7
            ? r => Gsm7Extension.Contains((char)r.Value) ? 2 : 1
            : r => r.Utf16SequenceLength;

        var parts = TextParts.Split(text, single, cost);
        if (parts.Count > 1) parts = TextParts.Split(text, segment, cost);

        var size = parts.Count == 1 ? $"1 SMS (up to {single} chars)" : $"{parts.Count} segments of up to {segment} chars";
        return new(parts, $"From {senderId}. {encoding}. {size}.");
    }

    public async Task<string> DeliverAsync(string recipient, string part)
    {
        await Task.Delay(60);
        return TextParts.ProviderId("sms");
    }
}
