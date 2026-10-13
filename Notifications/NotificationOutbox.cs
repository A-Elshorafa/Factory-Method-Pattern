using NotificationFactoryMethod.Models;

namespace NotificationFactoryMethod.Notifications;

/// <summary>Every dispatch, sent or rejected, newest first. Keeps the last <see cref="Capacity"/>.</summary>
public class NotificationOutbox
{
    private const int Capacity = 200;
    private readonly List<DispatchResult> _entries = [];
    private readonly Lock _lock = new();

    public List<DispatchResult> Entries
    {
        get { lock (_lock) return [.. _entries]; }
    }

    public void Add(DispatchResult result)
    {
        lock (_lock)
        {
            _entries.Insert(0, result);
            if (_entries.Count > Capacity) _entries.RemoveAt(_entries.Count - 1);
        }
    }

    public DispatchResult? Find(string id)
    {
        lock (_lock) return _entries.Find(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public void Clear()
    {
        lock (_lock) _entries.Clear();
    }
}
