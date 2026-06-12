namespace JobPortal.Shared.Exceptions;

internal class AccountLockedException : JPNSException
{
    public DateTime UnlockAt { get; }
    public AccountLockedException(DateTime unlockAt): base($"Account locked. Try again after {unlockAt:HH:mm:ss}.")
    {
        UnlockAt = unlockAt;
    }
}
