namespace NotificationFactoryMethod.Models;

/// <summary>What to say, before a channel turns it into the parts it actually sends.</summary>
public record Notification(string Title, string Body);
