using if_risk;
using if_risk.Exceptions;
using FluentAssertions;

namespace if_risk_tests;

public class RiskTests
{
    [Fact]
    public void CreateRisk_NameIsEmptyString_ThrowsInvalidRiskException()
    {
        Action act = () => new Risk("", 4);
        
        act.Should().Throw<InvalidRiskException>().WithMessage("Name cannot be null or empty string.");
    }
    
    [Fact]
    public void CreateRisk_NameIsNull_ThrowsInvalidRiskException()
    {
        Action act = () => new Risk(null, 4);
        
        act.Should().Throw<InvalidRiskException>().WithMessage("Name cannot be null or empty string.");
    }
    
    [Fact]
    public void CreateRisk_PriceEqualsOrIsLessThanZero_ThrowsInvalidRiskException()
    {
        Action act = () => new Risk("Fire", 0);
        
        act.Should().Throw<InvalidRiskException>().WithMessage("Price must be greater than 0.");
    }
    
    [Fact]
    public void CreateRisk_ValidRisk_CreatesRisk()
    {
        var risk = new Risk("Fire", 5);

        risk.Name.Should().Be("Fire");
        risk.YearlyPrice.Should().Be(5);
    }
}