using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class FixedRateRebateCalculator : IRebateCalculator
    {
        public bool CanCalculate(IncentiveType incentiveType) => incentiveType == IncentiveType.FixedRateRebate;

        public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (rebate == null || product == null || rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
                return 0;
            return product.Price * rebate.Percentage * request.Volume;
        }
    }
}
