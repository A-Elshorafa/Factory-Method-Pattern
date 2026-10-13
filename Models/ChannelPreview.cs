namespace NotificationFactoryMethod.Models;

public record ChannelPreview(NotificationChannel Channel, string Product, List<string> Parts, string Note, decimal Cost);
