using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Creator.</b> Builds an <see cref="SmsChannel"/> with the configured sender ID.</summary>
public class SmsDispatcher(NotificationOptions options) : NotificationDispatcher
{
    public override NotificationChannel Channel => NotificationChannel.Sms;

    protected override INotificationChannel CreateChannel() => new SmsChannel(options.SmsSenderId);
}
