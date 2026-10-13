namespace NotificationFactoryMethod.Models;

public record PreviewResult(int TitleLength, int BodyLength, List<ChannelPreview> Channels);
