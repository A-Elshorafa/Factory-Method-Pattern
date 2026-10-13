using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Creator.</b> Builds an <see cref="EmailChannel"/> with the configured From address.</summary>
public class EmailDispatcher(NotificationOptions options) : NotificationDispatcher
{
    public override NotificationChannel Channel => NotificationChannel.Email;

    protected override INotificationChannel CreateChannel() => new EmailChannel(options.EmailFrom);
}
