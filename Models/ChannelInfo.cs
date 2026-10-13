namespace NotificationFactoryMethod.Models;

public record ChannelInfo(
    NotificationChannel Channel,
    string Creator,
    string Product,
    string Description,
    string RecipientFormat,
    string ExampleRecipient,
    decimal CostPerPart);
