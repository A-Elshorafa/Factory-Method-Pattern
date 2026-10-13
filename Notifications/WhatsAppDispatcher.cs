using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Creator.</b> Builds a <see cref="WhatsAppChannel"/> with the configured business number.</summary>
public class WhatsAppDispatcher(NotificationOptions options) : NotificationDispatcher
{
    public override NotificationChannel Channel => NotificationChannel.WhatsApp;

    protected override INotificationChannel CreateChannel() => new WhatsAppChannel(options.WhatsAppNumber);
}
