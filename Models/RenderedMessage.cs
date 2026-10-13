namespace NotificationFactoryMethod.Models;

/// <summary>The parts a channel will deliver, and a note on how it got them (encoding, truncation, splitting).</summary>
public record RenderedMessage(List<string> Parts, string Note);
