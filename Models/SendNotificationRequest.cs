namespace NotificationFactoryMethod.Models;

public class SendNotificationRequest
{
    /// <example>Sms</example>
    public NotificationChannel Channel { get; set; }

    /// <summary>Its format depends on the channel. <c>GET /api/channels</c> lists an example for each.</summary>
    /// <example>+201012345678</example>
    public string Recipient { get; set; } = "";

    /// <example>Order #1042 shipped</example>
    public string Title { get; set; } = "";

    /// <example>Your order is on its way and should arrive on Thursday. Track it at nilemart.example/t/1042</example>
    public string Body { get; set; } = "";
}
