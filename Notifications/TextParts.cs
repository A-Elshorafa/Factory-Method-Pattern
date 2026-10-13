using System.Text;

namespace NotificationFactoryMethod.Notifications;

/// <summary>Cuts and splits text without breaking a character that takes two UTF-16 units (e.g. an emoji).</summary>
public static class TextParts
{
    public static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength) return text;
        var cut = maxLength - 1;
        if (char.IsHighSurrogate(text[cut - 1])) cut--;
        return text[..cut] + "…";
    }

    /// <summary>Splits into parts whose total <paramref name="cost"/> is at most <paramref name="capacity"/> each.</summary>
    public static List<string> Split(string text, int capacity, Func<Rune, int> cost)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        var used = 0;

        foreach (var rune in text.EnumerateRunes())
        {
            var size = cost(rune);
            if (used + size > capacity)
            {
                parts.Add(current.ToString());
                current.Clear();
                used = 0;
            }
            current.Append(rune);
            used += size;
        }

        if (current.Length > 0 || parts.Count == 0) parts.Add(current.ToString());
        return parts;
    }

    public static string ProviderId(string prefix) => $"{prefix}-{Guid.NewGuid().ToString("N")[..10]}";
}
