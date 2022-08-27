namespace if_risk.Exceptions;

public class InvalidPolicyException : Exception
{
    public InvalidPolicyException(string? message) : base(message)
    {
    }
}