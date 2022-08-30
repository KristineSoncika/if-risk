namespace if_risk;

public static class Premium
{
    // private readonly DateTime _start;
    // private readonly DateTime _end;
    // private readonly IList<Risk> _risks;
    private const int DaysInYear = 365;
    //
    // public PolicyPremium(DateTime start, DateTime end, IList<Risk> risks)
    // {
    //     _start = start;
    //     _end = end;
    //     _risks = risks;
    // }
    
    public static decimal CalculatePremium(DateTime startDate, DateTime endDate, decimal yearlyPrice)
    {
        return (endDate - startDate).Days * (yearlyPrice / DaysInYear);
    }
    
    // private readonly Policy _policy;
    // private readonly Risk _risk;
    // private readonly DateTime _riskStartDate;
    //
    // public Premium(IPolicy policy, Risk risk, DateTime start)
    // {
    //     _policy = policy as Policy;
    //     _risk = risk;
    //     _riskStartDate = start;
    // }
    //
    // public decimal CalculateAdditionalPremium()
    // {
    //     return (_policy.ValidTill - _riskStartDate).Days * (_risk.YearlyPrice / 365);
    // }
}