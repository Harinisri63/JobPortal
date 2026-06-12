namespace JobPortal.Features.Notifications;

internal class Notification
{
    private string   _notifId;
    private string   _userId;
    private string   _title;
    private string   _message;
    private string   _type;
    private bool     _isRead;
    private DateTime _createdAt;

    public string NotifId
    {
        get => _notifId;
        set => _notifId = value;
    }

    public string UserId
    {
        get => _userId;
        set => _userId = value;
    }

    public string Title
    {
        get => _title;
        set => _title = value;
    }

    public string Message
    {
        get => _message;
        set => _message = value;
    }

    public string Type
    {
        get => _type;
        set => _type = value;
    }

    public bool IsRead
    {
        get => _isRead;
        set => _isRead = value;
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value;
    }

    public Notification()
    {
        _notifId   = Guid.NewGuid().ToString();
        _userId    = "";
        _title     = "";
        _message   = "";
        _type      = "ANNOUNCEMENT";
        _isRead    = false;
        _createdAt = DateTime.Now;
    }

    public Notification(string userId, string title, string message, string type)
    {
        if (string.IsNullOrWhiteSpace(userId))   throw new ArgumentException("UserId is required.");
        if (string.IsNullOrWhiteSpace(message))  throw new ArgumentException("Message is required.");
        if (message.Length > 500)                throw new ArgumentException("Message must not exceed 500 chars.");
        if (title.Length > 100)                  throw new ArgumentException("Title must not exceed 100 chars.");

        _notifId   = Guid.NewGuid().ToString();
        _userId    = userId;
        _title     = title.Trim();
        _message   = message.Trim();
        _type      = type;
        _isRead    = false;
        _createdAt = DateTime.Now;
    }

    public void MarkRead() => _isRead = true;
    public bool IsUnread() => !_isRead;

    public void Display()
    {
        Console.Write($"  [{(_isRead ? " " : "*")}] ");
        Console.ForegroundColor = _isRead ? ConsoleColor.Gray : ConsoleColor.White;
        Console.WriteLine($"{_createdAt:dd MMM HH:mm}  {_title}");
        Console.ResetColor();
        Console.WriteLine($"       {_message}");
    }
}
