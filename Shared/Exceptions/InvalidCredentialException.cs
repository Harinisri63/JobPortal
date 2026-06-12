namespace JobPortal.Shared.Exceptions;

internal class InvalidCredentialException : JPNSException
{
    public int AttemptsRemaining { get; }
    public InvalidCredentialException(string message, int attemptsRemaining = 0): base(message)
    {
        AttemptsRemaining = attemptsRemaining;
    }
}
