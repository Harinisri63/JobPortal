namespace JobPortal.Shared.Exceptions;

internal class ValidationException : JPNSException
{
    public string FieldName { get; }
    public ValidationException(string fieldName, string message): base($"[{fieldName}] {message}")
    {
        FieldName = fieldName;
    }
}
