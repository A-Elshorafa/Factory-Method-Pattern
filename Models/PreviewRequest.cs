namespace NotificationFactoryMethod.Models;

public class PreviewRequest
{
    /// <example>Order #1042 shipped</example>
    public string Title { get; set; } = "";

    /// <example>Good news! Your order of 3 items has left our Cairo warehouse and is on its way with Nile Express. It should arrive on Thursday between 10:00 and 14:00. Track it any time at nilemart.example/t/1042 or reply HELP for support.</example>
    public string Body { get; set; } = "";

    /// <summary>Channels to render for. Leave empty to render for all of them.</summary>
    /// <example>[]</example>
    public List<NotificationChannel> Channels { get; set; } = [];
}
