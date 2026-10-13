using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary><b>Concrete Creator.</b> Builds a <see cref="PushChannel"/>. It needs no settings, unlike the other creators.</summary>
public class PushDispatcher : NotificationDispatcher
{
    public override NotificationChannel Channel => NotificationChannel.Push;

    protected override INotificationChannel CreateChannel() => new PushChannel();
}
