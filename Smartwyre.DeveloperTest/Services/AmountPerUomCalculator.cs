using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class AmountPerUomRebateCalculator : IRebateCalculator
    {
        public bool CanCalculate(IncentiveType incentiveType) => incentiveType == IncentiveType.AmountPerUom;

        public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (rebate == null || request.Volume == 0 || rebate.Amount == 0)
                return 0;
            return rebate.Amount * request.Volume;
        }
    }
}
