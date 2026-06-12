namespace JobPortal.Features.Candidate;

internal abstract class BaseUser
{
    protected string _id;
    protected string _email;
    protected string _passwordHash;
    protected bool   _isActive;
    protected DateTime _createdAt;
    protected DateTime _lastLoginAt;

    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public string Email
    {
        get => _email;
        set => _email = value;
    }

    public string PasswordHash
    {
        get => _passwordHash;
        set => _passwordHash = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value;
    }

    public DateTime LastLoginAt
    {
        get => _lastLoginAt;
        set => _lastLoginAt = value;
    }

    protected BaseUser()
    {
        _id           = "";
        _email        = "";
        _passwordHash = "";
        _isActive     = true;
        _createdAt    = DateTime.Now;
        _lastLoginAt  = DateTime.Now;
    }

    protected BaseUser(string id, string email, string passwordHash)
    {
        _id           = id;
        _email        = email.Trim().ToLowerInvariant();
        _passwordHash = passwordHash;
        _isActive     = true;
        _createdAt    = DateTime.Now;
        _lastLoginAt  = DateTime.Now;
    }

    public virtual void Login()  { _lastLoginAt = DateTime.Now; }
    public virtual void Logout() { }

    public abstract string GetRole();

    public void UpdateLastLogin() => _lastLoginAt = DateTime.Now;
}
