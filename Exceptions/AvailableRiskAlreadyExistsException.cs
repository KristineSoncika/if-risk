namespace if_risk.Exceptions;

public class AvailableRiskAlreadyExistsException : Exception
{
    public AvailableRiskAlreadyExistsException(string? risk) : base($"Risk name already exists: {risk}")
    {
        
    }
}