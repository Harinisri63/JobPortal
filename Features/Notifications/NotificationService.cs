using JobPortal.FileStorage;
using JobPortal.UI;

namespace JobPortal.Features.Notifications;

internal delegate void NotifyHandler(string message);

internal class NotificationService
{
    private readonly FileStorageService<Notification> _storage;
    private List<Notification> _notifications;

    public NotificationService()
    {
        _storage = new FileStorageService<Notification>("notifications.json");
        _notifications = _storage.LoadData();
    }

    public int GetPendingCount(string userId)
    {
        int count = 0;
        foreach (Notification notification in _notifications)
        {
            if (notification.UserId == userId &&
                notification.IsUnread())
            {
                count++;
            }
        }
        return count;
    }

    public List<Notification> GetPendingList(string userId)
    {
        List<Notification> result = new List<Notification>();
        foreach (Notification notification in _notifications)
        {
            if (notification.UserId == userId && notification.IsUnread())
            {
                result.Add(notification);
            }
        }
        result.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
        return result;
    }

    public List<Notification> GetAllForUser(string userId)
    {
        List<Notification> result = new List<Notification>();
        foreach (Notification notification in _notifications)
        {
            if (notification.UserId == userId)
            {
                result.Add(notification);
            }
        }
        result.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
        return result;
    }

    public void DisplayInbox(string userId)
    {
        var list = GetAllForUser(userId);
        int unread = GetPendingCount(userId);

        ConsoleHelper.PrintHeader($"Notifications  ({unread} unread)");
        if (list.Count == 0)
        {
            ConsoleHelper.PrintInfo("No notifications.");
            return;
        }
        foreach (var n in list)
            n.Display();
    }

    public void MarkAllRead(string userId)
    {
        foreach (Notification notification in _notifications)
        {
            if (notification.UserId == userId)
            {
                notification.MarkRead();
            }
        }
        Save();
    }

    public void Send(string userId, string title, string message, string type = "ANNOUNCEMENT")
    {
        var n = new Notification(userId, title, message, type);
        _notifications.Add(n);
        Save();
    }

    public void BroadcastToAll(IEnumerable<string> userIds, string title, string message)
    {
        foreach (var uid in userIds)
        {
            var n = new Notification(uid, title, message, "ANNOUNCEMENT");
            _notifications.Add(n);
        }
        Save();
    }

    public async Task SendNotificationAsync(string userId, string title, string message, string type = "ANNOUNCEMENT")
    {
        await Task.Run(() =>
        {
            var n = new Notification(userId, title, message, type);
            _notifications.Add(n);
            Save();
        });
    }

    public void Reload() => _notifications = DataStore.Notifications;
    private void Save()
    {
        _storage.SaveData(_notifications);
        if (JobPortal.Database.DatabaseSync.IsEnabled)
        {
            JobPortal.Database.DatabaseSync.SyncNotifications(_notifications);
        }
    }
}