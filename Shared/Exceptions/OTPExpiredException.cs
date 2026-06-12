namespace JobPortal.Shared.Exceptions;

internal class OTPExpiredException : JPNSException
{
    public OTPExpiredException(): base("OTP has expired. Please request a new one.") { }
}
