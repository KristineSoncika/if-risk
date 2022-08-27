namespace if_risk;

public class Premium
{
    private readonly DateTime _start;
    private readonly DateTime _end;
    private readonly IList<Risk> _risks;

    public Premium(DateTime start, DateTime end, IList<Risk> risks)
    {
        _start = start;
        _end = end;
        _risks = risks;
    }
    
    public decimal CalculateInitialPremium()
    {
        return _risks.Sum(risk => (_end - _start).Days * (risk.YearlyPrice / 365));
    }
    
    private readonly Policy _policy;
    private readonly Risk _risk;
    private readonly DateTime _riskStartDate;
    
    public Premium(IPolicy policy, Risk risk, DateTime start)
    {
        _policy = policy as Policy;
        _risk = risk;
        _riskStartDate = start;
    }

    public decimal CalculateAdditionalPremium()
    {
        return (_policy.ValidTill - _riskStartDate).Days * (_risk.YearlyPrice / 365);
    }
}