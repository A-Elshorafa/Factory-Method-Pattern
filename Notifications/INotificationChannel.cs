using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary>
/// <b>Product.</b> Knows one channel's rules: which recipients it accepts, how a notification fits into its
/// size limits, what each part costs, and how to deliver a part. The dispatcher only ever talks to this interface.
/// </summary>
public interface INotificationChannel
{
    string Description { get; }
    string RecipientFormat { get; }
    string ExampleRecipient { get; }

    /// <summary>In EGP. Made-up demo prices.</summary>
    decimal CostPerPart { get; }

    /// <summary>Returns why the recipient can't be used, or <c>null</c> if it can.</summary>
    string? ValidateRecipient(string recipient);

    RenderedMessage Render(Notification notification);

    /// <summary>Simulates the provider call. Returns the provider's message id.</summary>
    Task<string> DeliverAsync(string recipient, string part);
}
