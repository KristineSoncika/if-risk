using FluentAssertions;
using if_risk;

namespace if_risk_tests;

public class PolicyPremiumTests
{
    // [Fact]
    // public void CalculatePremium_ReturnsPolicyPremium()
    // {
    //     var start = DateTime.Today;
    //     var end = DateTime.Today.AddDays(3);
    //     var selectedRisks = new List<Risk>
    //     {
    //         new("Fire", 150),
    //         new("Steam leakage", 450)
    //     };
    //     const decimal premium = 3 * (150 / (decimal)365) + 3 * (450 / (decimal)365);
    //
    //     PolicyPremium.CalculatePremium(start, end, selectedRisks).Should().Be(premium);
    // }
    
    // [Fact]
    // public void CalculateAdditionalPremium_ReturnsAdditionalPremium()
    // {
    //     var start = DateTime.Today;
    //     var risk = new Risk("Theft", 350);
    //     var policy = new Policy("Bike-1", new DateTime(2022, 07, 01), start.AddDays(15), 50, new List<Risk>{new("Accident", 50)});
    //
    //     const decimal premium = 15 * (350 / (decimal)365);
    //
    //     new Premium(policy, risk, start).CalculateAdditionalPremium().Should().Be(premium);
    // }
}