using JobPortal.Shared.Exceptions;

namespace JobPortal.Features.Auth;

internal class OTPService
{
    private record OTPRecord(string Code, DateTime ExpiresAt, int Attempts);

    private readonly Dictionary<string, OTPRecord> _otpStore = new();
    private const int MaxAttempts = 3;
    private const int ExpiryMinutes = 10;

    public string GenerateOTP(string key)
    {
        string code = new Random().Next(100000, 999999).ToString();
        _otpStore[key] = new OTPRecord(code, DateTime.Now.AddMinutes(ExpiryMinutes), 0);
        return code;
    }

    public bool ValidateOTP(string key, string inputCode)
    {
        if (!_otpStore.TryGetValue(key, out var record))
            throw new OTPExpiredException();

        if (DateTime.Now > record.ExpiresAt)
        {
            _otpStore.Remove(key);
            throw new OTPExpiredException();
        }

        if (record.Attempts >= MaxAttempts)
        {
            _otpStore.Remove(key);
            throw new JPNSException("Maximum OTP attempts reached. Please request a new OTP.");
        }

        if (!record.Code.Equals(inputCode.Trim()))
        {
            _otpStore[key] = record with { Attempts = record.Attempts + 1 };
            throw new ValidationException("OTP", $"Invalid OTP. {MaxAttempts - record.Attempts - 1} attempt(s) remaining.");
        }

        _otpStore.Remove(key);
        return true;
    }

    public bool HasPendingOTP(string key) => _otpStore.ContainsKey(key);
}
