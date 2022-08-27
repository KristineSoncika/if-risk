using FluentAssertions;
using if_risk;
using if_risk.Exceptions;

namespace if_risk_tests;

public class InsuranceCompanyTests
{
    private InsuranceCompany _insuranceCompany;
    private string _insuranceCompanyName;
    private IList<Risk> _availableRisks;
    private List<Policy> _policies;

    private void Init()
    {
        _insuranceCompanyName = "If";
        _availableRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4),
            new("Natural disaster", 2)
        };
        _policies = new List<Policy>
        {
            new("Bike-1", new DateTime(2022, 06, 01), new DateTime(2022, 12, 31), 50, _availableRisks),
            new("Bike-2", DateTime.Today, DateTime.Today.AddMonths(6), 40, new List<Risk>{new("Fire", 3)}),
            new("Bike-3", DateTime.Today.AddMonths(3), DateTime.Today.AddMonths(6), 35, new List<Risk>{new("Fire", 3)}  )
        };
        _insuranceCompany = new InsuranceCompany(_insuranceCompanyName, _availableRisks, _policies);
    }
    
    [Fact]
    public void CreateCompany_ValidCompany_CreatesCompany()
    {
        Init();
        
        _insuranceCompany.Name.Should().Be(_insuranceCompanyName);
        _insuranceCompany.AvailableRisks.Should().BeEquivalentTo(_availableRisks);
        _insuranceCompany.Policies.Should().BeEquivalentTo(_policies);
    }

    [Fact]
    public void AddAvailableRisk_RiskNameIsNotUnique_ThrowsError()
    {
        Init();
        var risk = new Risk("Fire", 4);
        Action act = () => _insuranceCompany.UpdateAvailableRisks(risk);

        act.Should().Throw<AvailableRiskAlreadyExistsException>();
    }

    [Fact]
    public void AddAvailableRisk_RiskNameIsUnique_AddsRisk()
    {
        Init();
        var newRisk = new Risk("Theft", 4);
        
        _insuranceCompany.UpdateAvailableRisks(newRisk);
        var result = _insuranceCompany.AvailableRisks.Any(r => r.Name == newRisk.Name);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void GetRisks_ReturnsAvailableRisks()
    {
        Init();
        var result = _insuranceCompany.AvailableRisks;

        result.Should().BeEquivalentTo(_availableRisks);
    }
    
    [Fact]
    public void IsPolicyUniqueInGivenPeriod_NoPoliciesSoldYet_ReturnsTrue()
    {
        var policies = new List<Policy>();
        var insuranceCompany = new InsuranceCompany("If", _availableRisks, policies);
        const string name = "Bike-1";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddMonths(3);
        
        var result = insuranceCompany.IsPolicyUniqueInGivenPeriod(name, startDate, endDate);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsPolicyUniqueInGivenPeriod_EndDateIsBeforeValidFrom_ReturnsTrue()
    {
        Init();
        const string name = "Bike-3";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddMonths(1);

        var result = _insuranceCompany.IsPolicyUniqueInGivenPeriod(name, startDate, endDate);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsPolicyUniqueInGivenPeriod_EndDateIsAfterValidFrom_ReturnsFalse()
    {
        Init();
        const string name = "Bike-2";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddMonths(1);

        var result = _insuranceCompany.IsPolicyUniqueInGivenPeriod(name, startDate, endDate);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsPolicyUniqueInGivenPeriod_StartDateIsAfterValidTill_ReturnsTrue()
    {
        Init();
        const string name = "Bike-2";
        var startDate = DateTime.Today.AddMonths(7);
        var endDate = DateTime.Today.AddMonths(8);

        var result = _insuranceCompany.IsPolicyUniqueInGivenPeriod(name, startDate, endDate);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsPolicyUniqueInGivenPeriod_StartDateIsBeforeValidTill_ReturnsFalse()
    {
        Init();
        const string name = "Bike-1";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddMonths(3);

        var result = _insuranceCompany.IsPolicyUniqueInGivenPeriod(name, startDate, endDate);

        result.Should().BeFalse();
    }
    
    [Fact]
    public void AreSelectedRisksValid_AreValid_ReturnsTrue()
    {
        Init();
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4)
        };

        var result = _insuranceCompany.AreSelectedRisksValid(selectedRisks);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void AreSelectedRisksValid_AreNotValid_ReturnsFalse()
    {
        Init();
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4),
            new("Rain", 4)
        };

        var result = _insuranceCompany.AreSelectedRisksValid(selectedRisks);

        result.Should().BeFalse();
    }
    
    [Fact]
    public void SellPolicy_StartDateIsBeforeCurrentDate_ThrowsError()
    {
        Init();
        const string name = "Bike4";
        var startDate = new DateTime(2022, 07, 01);
        const short months = 4;

        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, _availableRisks);
        
        act.Should().Throw<InvalidDateException>().WithMessage("Start date cannot be less than current date.");
    }
    
    [Fact]
    public void SellPolicy_MonthsIsLessThanOne_ThrowsError()
    {
        Init();
        const string name = "Bike4";
        var startDate = DateTime.Today;
        const short months = 0;
        
        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, _availableRisks);

        act.Should().Throw<InvalidPolicyException>().WithMessage("Minimum number of months must be 1.");
    }

    [Fact]
    public void SellPolicy_SelectedRisksAreNotValid_ThrowsError()
    {
        Init();
        const string name = "Bike4";
        var startDate = DateTime.Today;
        const short months = 4;
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4),
            new("Theft", 4)
        };
        
        Action act = () => _insuranceCompany.SellPolicy(name, startDate, months, selectedRisks);

        act.Should().Throw<InvalidPolicyException>().WithMessage($"Risks can be selected from AvailableRisks list only: {_availableRisks}.");
    }

    [Fact]
    public void SellPolicy_PolicyIsValid_AddsPolicyToPolicies()
    {
        Init();
        const string name = "Bike4";
        var startDate = DateTime.Today;
        const short months = 4;
        var selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Steam leakage", 4)
        };
        
        var policy = _insuranceCompany.SellPolicy(name, startDate, months, selectedRisks);
        var result = _policies.Contains(policy);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void SellPolicy_PolicyIsValid_ReturnsPolicy()
    {
        Init();
        const string name = "Bike4";
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
        Init();
        Action act = () => _insuranceCompany.GetPolicy("Bike-4", new DateTime(2022, 02, 15));

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void GetPolicy_EffectiveDateIsOutOfRange_ThrowsError()
    {
        Init();
        Action act = () => _insuranceCompany.GetPolicy("Bike-3", DateTime.Today);

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void GetPolicy_PolicyFound_ReturnsPolicy()
    {
        Init();
        var policy = new Policy("Bike-2", DateTime.Today, DateTime.Today.AddMonths(6), 40, new List<Risk>{new("Fire", 3)});
        
        var result = _insuranceCompany.GetPolicy("Bike-2", DateTime.Today.AddDays(2));

        result.Should().BeEquivalentTo(policy);
    }

    [Fact]
    public void AddRisk_RiskNotInsured_ThrowsError()
    {
        Init();
        var risk = new Risk("Acid rain", 10);

        Action act = () => _insuranceCompany.AddRisk("Bike-2", risk, DateTime.Today);

        act.Should().Throw<RiskNotInsuredException>();
    }

    [Fact]
    public void AddRisk_PolicyNotFound_ThrowsError()
    {
        Init();
        var risk = new Risk("Fire", 10);

        Action act = () => _insuranceCompany.AddRisk("Bike-4", risk, DateTime.Today);

        act.Should().Throw<PolicyNotFoundException>();
    }
    
    [Fact]
    public void AddRisk_StartDateIsBeforeCurrentDate_ThrowsError()
    {
        Init();
        var risk = new Risk("Steam leakage", 4);

        Action act = () => _insuranceCompany.AddRisk("Bike-2", risk, new DateTime(2022, 08, 01));

        act.Should().Throw<InvalidDateException>().WithMessage("Start date cannot be less than current date.");
    }

    [Fact]
    public void AddRisk_RiskIsInsured_AddsRisk()
    {
        Init();
        var risk = new Risk("Steam leakage", 4);

        _insuranceCompany.AddRisk("Bike-2", risk, DateTime.Today);
        var result = _insuranceCompany.GetPolicy("Bike-2", DateTime.Today.AddDays(4));

        result.InsuredRisks.Should().Contain(risk);
    }
}