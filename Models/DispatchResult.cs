namespace NotificationFactoryMethod.Models;

public record DispatchResult(
    string Id,
    NotificationChannel Channel,
    string Creator,
    string Product,
    string Recipient,
    DispatchStatus Status,
    string? Error,
    List<DeliveredPart> Parts,
    decimal Cost,
    DateTime SentAt,
    List<DispatchStep> Steps);
