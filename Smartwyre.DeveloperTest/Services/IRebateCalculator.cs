using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public interface IRebateCalculator
    {
        bool CanCalculate(IncentiveType incentiveType);
        decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
    }
}
