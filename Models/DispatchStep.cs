namespace NotificationFactoryMethod.Models;

/// <summary>One thing that happened during a dispatch, and which participant of the pattern did it.</summary>
public record DispatchStep(string Actor, string Role, string Action);
