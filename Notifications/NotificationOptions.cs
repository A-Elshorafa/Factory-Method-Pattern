namespace NotificationFactoryMethod.Notifications;

/// <summary>Bound from the <c>Notifications</c> section of appsettings.json. Each concrete creator reads only what its channel needs.</summary>
public class NotificationOptions
{
    public string EmailFrom { get; set; } = "NileMart <no-reply@nilemart.example>";
    public string SmsSenderId { get; set; } = "NILEMART";
    public string WhatsAppNumber { get; set; } = "+20 2 2555 0100";
}
