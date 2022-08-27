using if_risk.Exceptions;

namespace if_risk;

public struct Risk
{
    /// <summary>
    /// Unique name of the risk
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Risk yearly price
    /// </summary>
    public decimal YearlyPrice { get; set; }

    public Risk(string name, decimal yearlyPrice)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidRiskException("Name cannot be null or empty string.");
        }
        
        if (yearlyPrice <= 0)
        {
            throw new InvalidRiskException("Price must be greater than 0.");
        }
        
        Name = name;
        YearlyPrice = yearlyPrice;
    }
}