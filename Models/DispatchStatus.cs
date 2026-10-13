using System.Text.Json.Serialization;

namespace NotificationFactoryMethod.Models;

[JsonConverter(typeof(JsonStringEnumConverter<DispatchStatus>))]
public enum DispatchStatus
{
    Sent,

    /// <summary>The channel refused the recipient, so nothing was sent.</summary>
    Rejected,
}
