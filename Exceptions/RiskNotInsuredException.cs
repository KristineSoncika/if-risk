namespace if_risk.Exceptions;

public class RiskNotInsuredException : Exception
{
    public RiskNotInsuredException(string? risk) : base($"This risk is not insured: {risk}. Select from available risks.")
    {
        
    }
}