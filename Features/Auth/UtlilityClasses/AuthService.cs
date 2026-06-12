using JobPortal.Shared.Exceptions;
using JobPortal.Features.Candidate;
using JobPortal.FileStorage;
using AdminNS = JobPortal.Features.Admin;

namespace JobPortal.Features.Auth;

internal static class AuthService
{
    private const int LockDurationMinutes = 30;
    private const int MaxFailedAttempts = 5;

    private static readonly Dictionary<string, DateTime> _lockTimestamps = new();

    public static string HashPassword(string rawPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(rawPassword, workFactor: 11);
    }

    public static bool VerifyPassword(string rawPassword, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(rawPassword, hash);
    }

    public static JobSeeker? LoginJobSeeker(string identifier, string rawPassword)
    {
        var seeker = DataStore.FindJobSeeker(identifier);
        if (seeker == null) return null;

        if (!seeker.IsActive)
        {
            if (_lockTimestamps.TryGetValue(seeker.SeekerId, out DateTime unlockAt) && DateTime.Now < unlockAt)
                throw new AccountLockedException(unlockAt);
            if (seeker.FailedAttempts >= MaxFailedAttempts)
            {
                DateTime unlock = DateTime.Now.AddMinutes(LockDurationMinutes);
                _lockTimestamps[seeker.SeekerId] = unlock;
                throw new AccountLockedException(unlock);
            }
        }

        if (!VerifyPassword(rawPassword, seeker.PasswordHash))
        {
            seeker.IncrementFailedAttempts();
            if (seeker.FailedAttempts >= MaxFailedAttempts)
            {
                seeker.Lock();
                DateTime unlock = DateTime.Now.AddMinutes(LockDurationMinutes);
                _lockTimestamps[seeker.SeekerId] = unlock;
                DataStore.SaveJobSeekers();
                throw new AccountLockedException(unlock);
            }
            DataStore.SaveJobSeekers();
            int remaining = MaxFailedAttempts - seeker.FailedAttempts;
            throw new InvalidCredentialException($"Invalid credentials. {remaining} attempt(s) remaining.", remaining);
        }
        seeker.ResetFailedAttempts();
        seeker.UpdateLastLogin();
        DataStore.SaveJobSeekers();
        return seeker;
    }

    public static AdminNS.Admin1? LoginAdmin(string email, string rawPassword)
    {
        var admin = DataStore.FindAdmin(email);
        if (admin == null) return null;
        if (!VerifyPassword(rawPassword, admin.PasswordHash))
            throw new InvalidCredentialException("Invalid admin credentials.");
        admin.UpdateLastLogin();
        DataStore.SaveAdmins();
        return admin;
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email", "Email is required.");
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ValidationException("Email", "Invalid email format. Example: user@gmail.com");
        if (email.Length > 100)
            throw new ValidationException("Email", "Email must not exceed 100 characters.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(email,@"^[a-zA-Z0-9._%+\-]+@(gmail\.com|jpns\.com|[a-zA-Z0-9.\-]+\.in\.edu)$",System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            throw new ValidationException("Email","Only @gmail.com, @jpns.com, and .in.edu email addresses are allowed.");
        }
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ValidationException("Phone", "Phone number is required.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10}$"))
            throw new ValidationException("Phone", "Phone must be exactly 10 digits.");
    }

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("Password", "Password is required.");
        if (password.Length < 8)
            throw new ValidationException("Password", "Password must be at least 8 characters.");
        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecial = false;
        string specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (specialChars.Contains(c)) hasSpecial = true;
        }
        if (!hasUpper)
            throw new ValidationException("Password", "Password must contain at least one uppercase letter.");
        if (!hasLower)
            throw new ValidationException("Password", "Password must contain at least one lowercase letter.");
        if (!hasDigit)
            throw new ValidationException("Password", "Password must contain at least one digit.");
        if (!hasSpecial)
            throw new ValidationException("Password", "Password must contain at least one special character.");
    }

    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name", "Name is required.");
        if (name.Trim().Length < 2 || name.Trim().Length > 80)
            throw new ValidationException("Name", "Name must be 2-80 characters.");
        bool isValid = true;
        foreach (char c in name)
        {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
            {
                isValid = false;
                break;
            }
        }
        if (!isValid)
        {
            throw new ValidationException("Name", "Name must contain only letters and spaces.");
        }
    }
}
