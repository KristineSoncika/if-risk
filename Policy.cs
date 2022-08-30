using if_risk.Exceptions;

namespace if_risk;

public class Policy : IPolicy
{
    public string NameOfInsuredObject { get; }
    public DateTime ValidFrom { get; }
    public DateTime ValidTill { get; }
    public IList<Risk> InsuredRisks { get; }
    public decimal Premium => InsuredRisks.Select(r => PolicyPremium.CalculatePremium
        (r.StartDate > ValidFrom ? r.StartDate : ValidFrom, ValidTill, r.YearlyPrice)).Sum();

    public Policy(string nameOfInsuredObject, DateTime validFrom, DateTime validTill, IList<Risk> insuredRisks)
    {
        if (string.IsNullOrEmpty(nameOfInsuredObject))
        {
            throw new InvalidPolicyException("Name cannot be null or empty string.");
        }

        if (validTill <= validFrom)
        {
            throw new InvalidPolicyException("End date must be greater start date.");
        }

        if (insuredRisks.Count < 1)
        {
            throw new InvalidPolicyException("A minimum of one risk must be selected.");
        }
        
        NameOfInsuredObject = nameOfInsuredObject;
        ValidFrom = validFrom;
        ValidTill = validTill;
        InsuredRisks = insuredRisks;
    }
}