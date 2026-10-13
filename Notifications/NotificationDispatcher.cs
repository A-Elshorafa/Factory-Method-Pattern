using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary>
/// <b>Creator.</b> Holds the steps every channel shares: validate the recipient, render, deliver each part, add up the cost.
/// It never names a concrete channel. It gets one from <see cref="CreateChannel"/>, the <b>factory method</b>,
/// which each subclass overrides to build its own <see cref="INotificationChannel"/>.
/// </summary>
public abstract class NotificationDispatcher
{
    private const string CreatorRole = "Creator", ConcreteCreatorRole = "Concrete Creator", ProductRole = "Product";

    public abstract NotificationChannel Channel { get; }

    /// <summary><b>The factory method.</b> Called on every dispatch, so each notification gets a new channel object.</summary>
    protected abstract INotificationChannel CreateChannel();

    public async Task<DispatchResult> DispatchAsync(string recipient, Notification notification)
    {
        var id = $"ntf-{Guid.NewGuid().ToString("N")[..8]}";
        var creator = GetType().Name;
        var product = CreateChannel();
        var productName = product.GetType().Name;

        List<DispatchStep> steps = [new(creator, ConcreteCreatorRole, $"CreateChannel() returned a new {productName}")];
        DispatchResult Result(DispatchStatus status, string? error, List<DeliveredPart> parts, decimal cost) =>
            new(id, Channel, creator, productName, recipient, status, error, parts, cost, DateTime.UtcNow, steps);

        if (product.ValidateRecipient(recipient) is { } error)
        {
            steps.Add(new(productName, ProductRole, $"ValidateRecipient(\"{recipient}\") rejected it: {error}"));
            steps.Add(new(nameof(NotificationDispatcher), CreatorRole, "Stopped. Nothing was sent or billed."));
            return Result(DispatchStatus.Rejected, error, [], 0);
        }
        steps.Add(new(productName, ProductRole, $"ValidateRecipient(\"{recipient}\") accepted it"));

        var rendered = product.Render(notification);
        steps.Add(new(productName, ProductRole, $"Render() produced {Plural(rendered.Parts.Count, "part")}. {rendered.Note}"));

        var delivered = new List<DeliveredPart>();
        for (var i = 0; i < rendered.Parts.Count; i++)
        {
            var part = rendered.Parts[i];
            var providerId = await product.DeliverAsync(recipient, part);
            delivered.Add(new(i + 1, part, part.Length, providerId));
            steps.Add(new(productName, ProductRole, $"DeliverAsync() part {i + 1}/{rendered.Parts.Count} ({part.Length} chars) → {providerId}"));
        }

        var cost = delivered.Count * product.CostPerPart;
        steps.Add(new(nameof(NotificationDispatcher), CreatorRole, $"{Plural(delivered.Count, "part")} × {product.CostPerPart:0.00} EGP = {cost:0.00} EGP"));
        return Result(DispatchStatus.Sent, null, delivered, cost);
    }

    /// <summary>Renders without sending, using the same factory method.</summary>
    public ChannelPreview Preview(Notification notification)
    {
        var product = CreateChannel();
        var rendered = product.Render(notification);
        return new(Channel, product.GetType().Name, rendered.Parts, rendered.Note, rendered.Parts.Count * product.CostPerPart);
    }

    public ChannelInfo Describe()
    {
        var product = CreateChannel();
        return new(Channel, GetType().Name, product.GetType().Name, product.Description, product.RecipientFormat, product.ExampleRecipient, product.CostPerPart);
    }

    private static string Plural(int count, string noun) => count == 1 ? $"1 {noun}" : $"{count} {noun}s";
}
