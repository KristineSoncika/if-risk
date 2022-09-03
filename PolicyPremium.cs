namespace if_risk;

public static class PolicyPremium
{
    private const int DaysInYear = 365;

    public static decimal CalculatePremium(DateTime startDate, DateTime endDate, decimal yearlyPrice)
    {
        return (endDate - startDate).Days * (yearlyPrice / DaysInYear);
    }
}