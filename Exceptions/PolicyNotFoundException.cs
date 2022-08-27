namespace if_risk.Exceptions;

public class PolicyNotFoundException : Exception
{
    public PolicyNotFoundException(string? name, DateTime date) : base($"Policy not found: {name} | {date}")
    {
        
    }
}