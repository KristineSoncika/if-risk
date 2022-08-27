using FluentAssertions;
using if_risk;
using if_risk.Exceptions;

namespace if_risk_tests;

public class PolicyTests
{
    private IList<Risk> _selectedRisks;

    private void Init()
    {
        _selectedRisks = new List<Risk>
        {
            new("Fire", 3),
            new("Natural disaster", 2)
        };
    }
    
    [Fact]
    public void CreatePolicy_NameIsEmptyString_ThrowsError()
    {
        Action act = () => new Policy("", DateTime.Today, DateTime.Today.AddMonths(3), 50, _selectedRisks);

        act.Should().Throw<InvalidPolicyException>().WithMessage("Name cannot be null or empty string.");
    }
    
    [Fact]
    public void CreatePolicy_NameIsNull_ThrowsError()
    {
        Action act = () => new Policy(null, DateTime.Today, DateTime.Today.AddMonths(3), 50, _selectedRisks);
        
        act.Should().Throw<InvalidPolicyException>().WithMessage("Name cannot be null or empty string.");
    }

    [Fact]
    public void CreatePolicy_EndDateIsLessThanStartDate_ThrowsError()
    {
        Action act = () => new Policy("Bike-1", DateTime.Today, new DateTime(2022, 08, 01), 50, _selectedRisks);
        
        act.Should().Throw<InvalidPolicyException>().WithMessage("End date must be greater start date.");
    }
    
    [Fact]
    public void CreatePolicy_PremiumEqualsOrIsLessThanZero_ThrowsError()
    {
        Action act = () => new Policy("Bike-1", DateTime.Today, DateTime.Today.AddMonths(2), 0, _selectedRisks);
        
        act.Should().Throw<InvalidPolicyException>().WithMessage("Premium must be greater than 0.");
    }
    
    [Fact]
    public void CreatePolicy_NoRisksSelected_ThrowsError()
    {
        Action act = () => new Policy("Bike-1", DateTime.Today, DateTime.Today.AddMonths(2), 50, new List<Risk>());
        
        act.Should().Throw<InvalidPolicyException>().WithMessage("A minimum of one risk must be selected.");
    }
    
    [Fact]
    public void CreatePolicy_ValidPolicy_CreatesPolicy()
    {
        Init();
        
        var policy = new Policy("Bike-1", DateTime.Today, DateTime.Today.AddMonths(2), 50, _selectedRisks);

        policy.NameOfInsuredObject.Should().Be("Bike-1");
        policy.ValidFrom.Should().Be(DateTime.Today);
        policy.ValidTill.Should().Be(DateTime.Today.AddMonths(2));
        policy.Premium.Should().Be(50);
        policy.InsuredRisks.Should().Equal(_selectedRisks);
    }
}