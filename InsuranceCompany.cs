using if_risk.Exceptions;

namespace if_risk;

public class InsuranceCompany : IInsuranceCompany
{
    public string Name { get; }
    public IList<Risk> AvailableRisks { get; set; }
    private readonly List<Policy> _policies;

    public InsuranceCompany(string name, IList<Risk> availableRisks, List<Policy> policies)
    {
        Name = name;
        AvailableRisks = availableRisks;
        _policies = policies;
    }
    
    private void ValidatePolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths, IEnumerable<Risk> selectedRisks)
    {
        if (validFrom < DateTime.Today)
        {
            throw new InvalidDateException("Start date cannot be less than current date.");
        }
        
        if (validMonths <= 0)
        {
            throw new InvalidPolicyException("Minimum number of months must be 1.");
        }

        if (!selectedRisks.All(sr => AvailableRisks.Any(ar => ar.Name == sr.Name)))
        {
            throw new InvalidPolicyException($"Risks can be selected from AvailableRisks list only: {AvailableRisks}.");
        }

        if (!(_policies.Count < 1 || 
            _policies.All(p => p.NameOfInsuredObject != nameOfInsuredObject) ||
            _policies.Any(p => p.NameOfInsuredObject == nameOfInsuredObject && validFrom.AddMonths(validMonths) < p.ValidFrom || validFrom > p.ValidTill)))
        {
            throw new InvalidPolicyException($"A policy has already been issued for the insured object in the given period: {nameOfInsuredObject}.");
        }
    }

    public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths, IList<Risk> selectedRisks)
    {
        ValidatePolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks);

        var premium = new Premium( validFrom, validFrom.AddMonths(validMonths).AddDays(-1), selectedRisks);
        var policy = new Policy(nameOfInsuredObject, validFrom, validFrom.AddMonths(validMonths).AddDays(-1), premium.CalculateInitialPremium(), selectedRisks);
        
        _policies.Add(policy);
        return policy;
    }

    public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom)
    {
        if (AvailableRisks.All(r => r.Name != risk.Name))
        {
            throw new RiskNotInsuredException(risk.Name);
        }
        if (validFrom < DateTime.Today)
        {
            throw new InvalidDateException("Start date cannot be less than current date.");
        }
        
        var updatedPolicy = GetPolicy(nameOfInsuredObject, validFrom);
        updatedPolicy.InsuredRisks.Add(risk);
        var premium = new Premium(updatedPolicy, risk, validFrom);
        updatedPolicy.Premium += premium.CalculateAdditionalPremium();
    }

    public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
    {
        var policy = _policies.Find(p =>
            p.NameOfInsuredObject == nameOfInsuredObject && effectiveDate >= p.ValidFrom && effectiveDate <= p.ValidTill);

        if (policy == null)
        {
            throw new PolicyNotFoundException(nameOfInsuredObject, effectiveDate);
        }

        return policy;
    }
}