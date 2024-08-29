using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class FixedCashAmountRebateCalculator : IRebateCalculator
    {
        public bool CanCalculate(IncentiveType incentiveType) => incentiveType == IncentiveType.FixedCashAmount;

        public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (rebate == null || product == null || rebate.Amount == 0)
                return 0;
            return rebate.Amount;
        }
    }
}
