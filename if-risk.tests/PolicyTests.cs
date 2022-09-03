using FluentAssertions;
using if_risk;
using if_risk.Exceptions;

namespace if_risk_tests;

public class PolicyTests
{
    private readonly IList<Risk> _selectedRisks;

    public PolicyTests()
    {
        _selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Natural disaster", 2)
        };
    }
    
    [Fact]
    public void CreatePolicy_NameIsEmptyString_ThrowsInvalidPolicyException()
    {
        Action act = () => 
            new Policy("", DateTime.Today, DateTime.Today.AddMonths(3), _selectedRisks);

        act.Should().Throw<InvalidPolicyException>()
            .WithMessage("Name cannot be null or empty string.");
    }
    
    [Fact]
    public void CreatePolicy_NameIsNull_ThrowsInvalidPolicyException()
    {
        Action act = () => 
            new Policy(null, DateTime.Today, DateTime.Today.AddMonths(3), _selectedRisks);
        
        act.Should().Throw<InvalidPolicyException>()
            .WithMessage("Name cannot be null or empty string.");
    }

    [Fact]
    public void CreatePolicy_EndDateIsLessThanStartDate_ThrowsInvalidPolicyException()
    {
        Action act = () => 
            new Policy("Bike-1", DateTime.Today, new DateTime(2022, 08, 01), _selectedRisks);
        
        act.Should().Throw<InvalidPolicyException>()
            .WithMessage("End date must be greater start date.");
    }

    [Fact]
    public void CreatePolicy_NoRisksSelected_ThrowsInvalidPolicyException()
    {
        Action act = () => 
            new Policy("Bike-1", DateTime.Today, DateTime.Today.AddMonths(2), new List<Risk>());
        
        act.Should().Throw<InvalidPolicyException>()
            .WithMessage("A minimum of one risk must be selected.");
    }
    
    [Fact]
    public void CreatePolicy_ValidPolicy_CreatesPolicy()
    {
        const string name = "Bike-1";
        var start = DateTime.Today;
        var end = DateTime.Today.AddDays(3);
        var selectedRisks = new List<Risk>
        {
            new("Fire", 150),
            new("Steam leakage", 450)
        };
        var policy = new Policy(name, start, end, selectedRisks);
        var premium = policy.InsuredRisks.Select(r => PolicyPremium.CalculatePremium(start, end, r.YearlyPrice)).Sum();

        policy.NameOfInsuredObject.Should().Be("Bike-1");
        policy.ValidFrom.Should().Be(DateTime.Today);
        policy.ValidTill.Should().Be(DateTime.Today.AddDays(3));
        policy.Premium.Should().Be(premium);
        policy.InsuredRisks.Should().Equal(selectedRisks);
    }
}