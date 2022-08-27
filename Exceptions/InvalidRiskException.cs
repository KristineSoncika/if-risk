namespace if_risk.Exceptions;

public class InvalidRiskException : Exception
{
    public InvalidRiskException(string? message) : base(message)
    {
    }
}