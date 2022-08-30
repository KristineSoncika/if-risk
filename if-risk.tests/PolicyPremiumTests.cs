using FluentAssertions;
using if_risk;

namespace if_risk_tests;

public class PolicyPremiumTests
{
    [Fact]
    public void CalculatePremium_ReturnsPolicyPremium()
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
        const decimal premium = 3 * (150 / (decimal)365) + 3 * (450 / (decimal)365);

        var result = policy.InsuredRisks.Select(r => PolicyPremium.CalculatePremium(start, end, r.YearlyPrice)).Sum();

        result.Should().Be(premium);
    }
}