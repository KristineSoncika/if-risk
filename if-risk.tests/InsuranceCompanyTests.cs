using FluentAssertions;
using if_risk;
using if_risk.Exceptions;

namespace if_risk_tests;

public class InsuranceCompanyTests
{
    private readonly IInsuranceCompany _insuranceCompany;
    private readonly string _insuranceCompanyName;
    private readonly IList<Risk> _availableRisksList;
    private readonly List<Policy> _policiesList;

    public InsuranceCompanyTests()
    {
        _insuranceCompanyName = "If";
        _availableRisksList = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4),
            new("Natural disaster", 2)
        };
        _policiesList = new List<Policy>
        {
            new("Bike-1", new DateTime(2022, 06, 01), new DateTime(2022, 12, 31), 50, _availableRisksList),
            new("Bike-2", DateTime.Today, DateTime.Today.AddMonths(6), 40, new List<Risk>{new("Fire", 3)}),
            new("Bike-3", DateTime.Today.AddMonths(3), DateTime.Today.AddMonths(6), 35, new List<Risk>{new("Fire", 3)}  )
        };
        _insuranceCompany = new InsuranceCompany(_insuranceCompanyName, _availableRisksList, _policiesList);
    }

    [Fact]
    public void CreateCompany_ValidCompany_CreatesCompany()
    {
        _insuranceCompany.Name.Should().Be(_insuranceCompanyName);
        _insuranceCompany.AvailableRisks.Should().BeEquivalentTo(_availableRisksList);
    }

    [Fact]
    public void GetRisks_ReturnsAvailableRisks()
    {
        var result = _insuranceCompany.AvailableRisks;
        
        result.Should().BeEquivalentTo(_availableRisksList);
    }

    [Fact]
    public void AddAvailableRisk_AddsRisk()
    {
        var newRisk = new Risk("Theft", 10);
        
        _insuranceCompany.AvailableRisks.Add(newRisk);
        _insuranceCompany.AvailableRisks.Should().Contain(newRisk);
    }
    
    [Fact]
    public void SellPolicy_PolicyNotUnique_ThrowsError()
    {
        const string name = "Bike-1";
        var startDate = DateTime.Today;
        const short months = 4;
    
        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, _availableRisksList);

        act.Should().Throw<InvalidPolicyException>()
            .WithMessage($"A policy has already been issued for the insured object in the given period: {name}.");
    }

    [Fact]
    public void SellPolicy_StartDateIsBeforeCurrentDate_ThrowsError()
    {
        const string name = "Bike-4";
        var startDate = new DateTime(2022, 07, 01);
        const short months = 4;

        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, _availableRisksList);
        
        act.Should().Throw<InvalidDateException>()
            .WithMessage("Start date cannot be less than current date.");
    }
    
    [Fact]
    public void SellPolicy_MonthsIsLessThanOne_ThrowsError()
    {
        const string name = "Bike-4";
        var startDate = DateTime.Today;
        const short months = 0;
        
        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, _availableRisksList);

        act.Should().Throw<InvalidPolicyException>()
            .WithMessage("Minimum number of months must be 1.");
    }

    [Fact]
    public void SellPolicy_SelectedRisksAreNotValid_ThrowsError()
    {
        const string name = "Bike-4";
        var startDate = DateTime.Today;
        const short months = 4;
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4),
            new("Theft", 4)
        };
        
        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, selectedRisks);

        act.Should().Throw<InvalidPolicyException>()
            .WithMessage($"Risks can be selected from AvailableRisks list only: {_availableRisksList}.");
    }

    [Fact]
    public void SellPolicy_PolicyIsValid_ReturnsPolicy()
    {
        const string name = "Bike-4";
        var startDate = DateTime.Today;
        const short months = 4;
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4)
        };
        var premium = new Premium( startDate, startDate.AddMonths(months).AddDays(-1), selectedRisks);
        var policy = new Policy(name, startDate, startDate.AddMonths(months).AddDays(-1), premium.CalculateInitialPremium(), selectedRisks);
        
        var result = _insuranceCompany.SellPolicy(name, startDate, months, selectedRisks);

        result.Should().BeEquivalentTo(policy);
    }

    [Fact]
    public void GetPolicy_InsuredObjectNameNotFound_ThrowsError()
    {
        Action act = () => _insuranceCompany.GetPolicy("Bike-4", new DateTime(2022, 02, 15));

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void GetPolicy_EffectiveDateIsOutOfRange_ThrowsError()
    {
        Action act = () => _insuranceCompany.GetPolicy("Bike-3", DateTime.Today);

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void GetPolicy_PolicyFound_ReturnsPolicy()
    {
        var policy = new Policy("Bike-2", DateTime.Today, DateTime.Today.AddMonths(6), 40, new List<Risk>{new("Fire", 3)});
        
        var result = _insuranceCompany.GetPolicy("Bike-2", DateTime.Today.AddDays(2));

        result.Should().BeEquivalentTo(policy);
    }

    [Fact]
    public void AddRisk_RiskNotInsured_ThrowsError()
    {
        var risk = new Risk("Acid rain", 10);

        Action act = () => _insuranceCompany.AddRisk("Bike-2", risk, DateTime.Today);

        act.Should().Throw<RiskNotInsuredException>();
    }

    [Fact]
    public void AddRisk_PolicyNotFound_ThrowsError()
    {
        var risk = new Risk("Fire", 10);

        Action act = () => _insuranceCompany.AddRisk("Bike-4", risk, DateTime.Today);

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void AddRisk_StartDateIsBeforeCurrentDate_ThrowsError()
    {
        var risk = new Risk("Steam leakage", 4);

        Action act = () => _insuranceCompany.AddRisk("Bike-2", risk, new DateTime(2022, 08, 01));

        act.Should().Throw<InvalidDateException>()
            .WithMessage("Start date cannot be less than current date.");
    }

    [Fact]
    public void AddRisk_RiskIsInsured_AddsRisk()
    {
        var risk = new Risk("Steam leakage", 4);

        _insuranceCompany.AddRisk("Bike-2", risk, DateTime.Today);
        var result = _insuranceCompany.GetPolicy("Bike-2", DateTime.Today.AddDays(4));

        result.InsuredRisks.Should().Contain(risk);
    }
}