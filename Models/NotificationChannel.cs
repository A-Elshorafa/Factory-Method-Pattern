using System.Text.Json.Serialization;

namespace NotificationFactoryMethod.Models;

/// <summary>A channel a notification can be sent through. Each one has its own dispatcher (Concrete Creator).</summary>
[JsonConverter(typeof(JsonStringEnumConverter<NotificationChannel>))]
public enum NotificationChannel
{
    /// <summary>One message with a subject line. No length limit.</summary>
    Email,

    /// <summary>Split into 153-char (GSM-7) or 67-char (UCS-2) segments, each billed separately.</summary>
    Sms,

    /// <summary>Title and body are cut to fit the notification banner. Free.</summary>
    Push,

    /// <summary>Bold title, split at 4096 chars. Needs the international number format.</summary>
    WhatsApp,
}
